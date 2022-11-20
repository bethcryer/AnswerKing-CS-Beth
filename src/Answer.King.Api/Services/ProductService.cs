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
        var productCategories = new List<CategoryId>();
        var categories = new List<Domain.Inventory.Category>();

        foreach (var categoryId in createProduct.Categories)
        {
            var category = await this.Categories.Get(categoryId);

            if (category == null)
            {
                throw new ProductServiceException("The provided category id is not valid.");
            }

            productCategories.Add(new CategoryId(category.Id));
            categories.Add(category);
        }

        var product = new Product(
            createProduct.Name,
            createProduct.Description,
            createProduct.Price,
            productCategories);

        await this.Products.AddOrUpdate(product);

        foreach (var category in categories)
        {
            category.AddProduct(new ProductId(product.Id));
            await this.Categories.Save(category);
        }

        return product;
    }

    public async Task<Product?> UpdateProduct(long productId, RequestModels.Product updateProduct)
    {
        var product = await this.Products.Get(productId);

        if (product == null)
        {
            return null;
        }

        var oldCategories = await this.Categories.GetByProductId(productId);

        if (!oldCategories.Any())
        {
            throw new ProductServiceException("Could not find any categories for this product id.");
        }

        foreach (var oldCategory in oldCategories.ToList())
        {
            // If old category is not still present in updated list, remove link between product and category.
            if (!updateProduct.Categories.Contains(oldCategory.Id))
            {
                oldCategory.RemoveProduct(new ProductId(productId));
                await this.Categories.Save(oldCategory);

                product.RemoveCategory(new CategoryId(oldCategory.Id));
                continue;
            }

            // Else check that the category is still in the database.
            var category = await this.Categories.Get(oldCategory.Id);

            if (category == null)
            {
                throw new ProductServiceException("The provided category id is not valid.");
            }

            category.AddProduct(new ProductId(productId));
            await this.Categories.Save(category);

            product.AddCategory(new CategoryId(category.Id));

            updateProduct.Categories.Remove(oldCategory.Id);
        }

        // Add any new categories remaining in the list
        foreach (var updateId in updateProduct.Categories)
        {
            var category = await this.Categories.Get(updateId);

            if (category == null)
            {
                throw new ProductServiceException("The provided category id is not valid.");
            }

            product.AddCategory(new CategoryId(category.Id));
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
