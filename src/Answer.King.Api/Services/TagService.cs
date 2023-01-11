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
        return await this.Tags.Get(tagId);
    }

    public async Task<IEnumerable<Tag>> GetTags()
    {
        return await this.Tags.Get();
    }

    public async Task<Tag> CreateTag(RequestModels.Tag createTag)
    {
        var tag = new Tag(createTag.Name, createTag.Description, new List<ProductId>());

        await this.Tags.Save(tag);

        return tag;
    }

    public async Task<Tag?> UpdateTag(long tagId, RequestModels.Tag updateTag)
    {
        var tag = await this.Tags.Get(tagId);
        if (tag == null)
        {
            return null;
        }

        tag.Rename(updateTag.Name, updateTag.Description);

        await this.Tags.Save(tag);

        return tag;
    }

    public async Task<Tag?> RetireTag(long tagId)
    {
        var tag = await this.Tags.Get(tagId);
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

    public async Task<Tag?> AddProducts(long tagId, RequestModels.TagProducts addProducts)
    {
        var tag = await this.Tags.Get(tagId);
        if (tag == null)
        {
            return null;
        }

        foreach (var productId in addProducts.Products)
        {
            var product = await this.Products.Get(productId);

            if (product == null)
            {
                throw new TagServiceException("The provided product id is not valid.");
            }

            tag.AddProduct(new(product.Id));

            product.AddTag(new TagId(tag.Id));
            await this.Products.AddOrUpdate(product);
        }

        await this.Tags.Save(tag);

        return tag;
    }

    public async Task<Tag?> RemoveProducts(long tagId, RequestModels.TagProducts removeProducts)
    {
        var tag = await this.Tags.Get(tagId);
        if (tag == null)
        {
            return null;
        }

        foreach (var productId in removeProducts.Products)
        {
            var product = await this.Products.Get(productId);

            if (product == null)
            {
                throw new TagServiceException("The provided product id is not valid.");
            }

            tag.RemoveProduct(new(product.Id));

            product.RemoveTag(new TagId(tag.Id));
            await this.Products.AddOrUpdate(product);
        }

        await this.Tags.Save(tag);

        return tag;
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
