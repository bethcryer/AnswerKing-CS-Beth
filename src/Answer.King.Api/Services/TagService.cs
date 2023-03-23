using System.Runtime.Serialization;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Api.Services;

public class TagService : ITagService
{
    public TagService(
        ITagRepository tags,
        IProductRepository products)
    {
        this.Tags = tags;
        this.Products = products;
    }

    private ITagRepository Tags { get; }

    private IProductRepository Products { get; }

    public async Task<Tag?> GetTag(long tagId)
    {
        return await this.Tags.GetOne(tagId);
    }

    public async Task<Tag?> GetTagByName(string name)
    {
        return await this.Tags.GetOne(name);
    }

    public async Task<IEnumerable<Tag>> GetTags()
    {
        return await this.Tags.GetAll();
    }

    public async Task<Tag> CreateTag(RequestModels.Tag createTag)
    {
        var tag = new Tag(
            createTag.Name,
            createTag.Description,
            new List<ProductId>());

        try
        {
            this.Tags.BeginTransaction();
            await this.Tags.Save(tag);

            var products = await this.AssociateTagAndProducts(tag, createTag.Products);

            foreach (var product in products)
            {
                await this.Products.AddOrUpdate(product);
            }

            await this.Tags.Save(tag);
            this.Tags.CommitTransaction();

            return tag;
        }
        catch
        {
            this.Tags.RollbackTransaction();
            throw;
        }
    }

    public async Task<Tag?> UpdateTag(long tagId, RequestModels.Tag updateTag)
    {
        var tag = await this.Tags.GetOne(tagId);

        if (tag == null)
        {
            return null;
        }

        tag.Rename(updateTag.Name, updateTag.Description);
        var tagProductIdsToRemove = tag.Products.Where(x => !updateTag.Products.Contains(x));
        var updatedProducts = new List<Product>();

        foreach (var productIdToRemove in tagProductIdsToRemove)
        {
            var product = await this.Products.GetOne(productIdToRemove);

            try
            {
                tag.RemoveProduct(productIdToRemove);
                product!.RemoveTag(new TagId(tag.Id));
            }
            catch (Exception ex) when (ex is TagLifecycleException or ProductLifecycleException)
            {
                throw new TagServiceException(ex.Message, ex);
            }

            updatedProducts.Add(product);
        }

        updatedProducts.AddRange(await this.AssociateTagAndProducts(tag, updateTag.Products));

        foreach (var product in updatedProducts)
        {
            await this.Products.AddOrUpdate(product);
        }

        await this.Tags.Save(tag);

        return tag;
    }

    public async Task<Tag?> RetireTag(long tagId)
    {
        var tag = await this.Tags.GetOne(tagId);
        if (tag == null)
        {
            return null;
        }

        try
        {
            tag.RetireTag();

            await this.Tags.Save(tag);

            return tag;
        }
        catch (TagLifecycleException ex)
        {
            throw new TagServiceException(ex.Message, ex);
        }
    }

    public async Task<Tag?> UnretireTag(long tagId)
    {
        var tag = await this.Tags.GetOne(tagId);
        if (tag == null)
        {
            return null;
        }

        try
        {
            tag.UnretireTag();

            await this.Tags.Save(tag);

            return tag;
        }
        catch (TagLifecycleException ex)
        {
            throw new TagServiceException(ex.Message, ex);
        }
    }

    private async Task<List<Product>> AssociateTagAndProducts(Tag tag, List<long> products)
    {
        var updatedProducts = new List<Product>();

        foreach (var productId in products)
        {
            var product = await this.Products.GetOne(productId) ??
                          throw new TagServiceException($"The provided product id is not valid: {productId}");

            try
            {
                tag.AddProduct(new ProductId(product.Id));
                product.AddTag(new TagId(tag.Id));
            }
            catch (Exception ex) when (ex is TagLifecycleException or ProductLifecycleException)
            {
                throw new TagServiceException(ex.Message, ex);
            }

            updatedProducts.Add(product);
        }

        return updatedProducts;
    }
}

[Serializable]
public class TagServiceException : Exception
{
    public TagServiceException()
    {
    }

    public TagServiceException(string message)
        : base(message)
    {
    }

    public TagServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected TagServiceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
