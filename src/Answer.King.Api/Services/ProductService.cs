using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Api.Services;

public class ProductService : IProductService
{
    public ProductService(
        IProductRepository products,
        ICategoryRepository categories)
    {
        this.Products = products;
        this.Categories = categories;
    }

    private IProductRepository Products { get; }

    private ICategoryRepository Categories { get; }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await this.Products.Get();
    }

    public async Task<IEnumerable<Product>> GetProducts(IEnumerable<long> productIds)
    {
        return await this.Products.Get(productIds);
    }

    public async Task<Product?> GetProduct(long productId)
    {
        return await this.Products.Get(productId);
    }

    public async Task<Product> CreateProduct(RequestModels.Product createProduct)
    {
        var product = new Product(
            createProduct.Name,
            createProduct.Description,
            createProduct.Price);

        await this.Products.AddOrUpdate(product);

        return product;
    }

    public async Task<Product?> UpdateProduct(long productId, RequestModels.Product updateProduct)
    {
        var product = await this.Products.Get(productId);

        if (product == null)
        {
            return null;
        }

        product.Name = updateProduct.Name;
        product.Description = updateProduct.Description;
        product.Price = updateProduct.Price;

        await this.Products.AddOrUpdate(product);

        return product;
    }

    public async Task<Product?> RetireProduct(long productId)
    {
        var product = await this.Products.Get(productId);

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
}

[Serializable]
internal class ProductServiceException : Exception
{
    public ProductServiceException(string message) : base(message)
    {
    }

    public ProductServiceException() : base()
    {
    }

    public ProductServiceException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
