using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Api.Services;

using System.Runtime.Serialization;

public class ProductService : IProductService
{
    public ProductService(
        IProductRepository products,
        ICategoryRepository categories,
        ITagRepository tags)
    {
        this.Products = products;
        this.Categories = categories;
        this.Tags = tags;
    }

    private IProductRepository Products { get; }

    private ICategoryRepository Categories { get; }

    private ITagRepository Tags { get; }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await this.Products.GetAll();
    }

    public async Task<IEnumerable<Product>> GetProducts(IEnumerable<long> productIds)
    {
        return await this.Products.GetMany(productIds);
    }

    public async Task<Product?> GetProduct(long productId)
    {
        return await this.Products.GetOne(productId);
    }

    public async Task<Product?> GetProductByName(string name)
    {
        return await this.Products.GetOne(name);
    }

    public async Task<Product> CreateProduct(RequestModels.Product createProduct)
    {
        var category = await this.Categories.GetOne(createProduct.CategoryId) ??
                       throw new ProductServiceException("The provided product category Id is not valid.");

        var product = new Product(
            createProduct.Name,
            createProduct.Description,
            createProduct.Price,
            new ProductCategory(category.Id, category.Name, category.Description));

        this.Products.BeginTransaction();

        await this.Products.AddOrUpdate(product);
        category.AddProduct(new ProductId(product.Id));

        await this.Categories.Save(category);

        try
        {
            this.Tags.BeginTransaction();

            var tags = await this.AssociateProductAndTags(product, createProduct.Tags);

            foreach (var tag in tags)
            {
                await this.Tags.Save(tag);
            }

            this.Tags.CommitTransaction();
        }
        catch
        {
            this.Tags.RollbackTransaction();
            this.Products.RollbackTransaction();
            throw;
        }

        this.Products.CommitTransaction();

        return product;
    }

    public async Task<Product?> UpdateProduct(long productId, RequestModels.Product updateProduct)
    {
        var product = await this.Products.GetOne(productId);

        if (product == null)
        {
            return null;
        }

        if (product.Retired)
        {
            throw new ProductServiceException("The product is retired.");
        }

        product.Name = updateProduct.Name;
        product.Description = updateProduct.Description;
        product.Price = updateProduct.Price;

        if (product.Category.Id != updateProduct.CategoryId)
        {
            var category = await this.Categories.GetOne(updateProduct.CategoryId) ??
                           throw new ProductServiceException("The provided product category Id is not valid.");

            var currentCategory = await this.Categories.GetOne(product.Category.Id) ??
                                  throw new ProductServiceException("The current category is not valid");

            currentCategory.RemoveProduct(new ProductId(product.Id));

            await this.Categories.Save(currentCategory);

            product.SetCategory(new ProductCategory(category.Id, category.Name, category.Description));

            category.AddProduct(new ProductId(product.Id));

            await this.Categories.Save(category);
        }

        var tagIdsToRemove = product.Tags.Where(x => !updateProduct.Tags.Contains(x));
        var updatedTags = new List<Tag>();

        foreach (var tagId in tagIdsToRemove)
        {
            var tag = await this.Tags.GetOne(tagId);

            try
            {
                product.RemoveTag(tagId);
                tag!.RemoveProduct(new ProductId(productId));
            }
            catch (Exception ex) when (ex is TagLifecycleException or ProductLifecycleException)
            {
                throw new ProductServiceException(ex.Message, ex);
            }

            updatedTags.Add(tag);
        }

        updatedTags.AddRange(await this.AssociateProductAndTags(product, updateProduct.Tags));

        try
        {
            this.Tags.BeginTransaction();

            foreach (var tag in updatedTags)
            {
                await this.Tags.Save(tag);
            }

            this.Tags.CommitTransaction();
        }
        catch
        {
            this.Tags.RollbackTransaction();
            throw;
        }

        await this.Products.AddOrUpdate(product);

        return product;
    }

    public async Task<Product?> RetireProduct(long productId)
    {
        var product = await this.Products.GetOne(productId);

        if (product == null)
        {
            return null;
        }

        if (product.Retired)
        {
            throw new ProductServiceException("The product is already retired.");
        }

        var categories = await this.Categories.GetByProductId(productId);
        foreach (var category in categories.ToList())
        {
            category.RemoveProduct(new ProductId(productId));
            await this.Categories.Save(category);
        }

        product.Retire();

        await this.Products.AddOrUpdate(product);

        return product;
    }

    public async Task<Product?> UnretireProduct(long productId)
    {
        var product = await this.Products.GetOne(productId);

        if (product == null)
        {
            return null;
        }

        if (!product.Retired)
        {
            throw new ProductServiceException("The product is not retired.");
        }

        product.Unretire();

        await this.Products.AddOrUpdate(product);

        return product;
    }

    private async Task<List<Tag>> AssociateProductAndTags(Product product, List<long> tags)
    {
        var updatedTags = new List<Tag>();

        foreach (var tagId in tags)
        {
            var tag = await this.Tags.GetOne(tagId) ??
                            throw new ProductServiceException($"The provided tag id is not valid: {tagId}");

            try
            {
                tag.AddProduct(new ProductId(product.Id));
                product.AddTag(new TagId(tagId));
            }
            catch (Exception ex) when (ex is ProductLifecycleException or TagLifecycleException)
            {
                throw new ProductServiceException(ex.Message, ex);
            }

            updatedTags.Add(tag);
        }

        return updatedTags;
    }
}

[Serializable]
public class ProductServiceException : Exception
{
    public ProductServiceException(string message)
        : base(message)
    {
    }

    public ProductServiceException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected ProductServiceException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}
