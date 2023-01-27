using System.Runtime.Serialization;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;
using Category = Answer.King.Domain.Inventory.Category;

namespace Answer.King.Api.Services;

public class CategoryService : ICategoryService
{
    public CategoryService(
        ICategoryRepository categories,
        IProductRepository products)
    {
        this.Categories = categories;
        this.Products = products;
    }

    private ICategoryRepository Categories { get; }

    private IProductRepository Products { get; }

    public async Task<Category?> GetCategory(long categoryId)
    {
        return await this.Categories.GetOne(categoryId);
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await this.Categories.GetAll();
    }

    public async Task<Category> CreateCategory(RequestModels.Category createCategory)
    {
        var categoryProducts = new List<ProductId>();
        var products = new List<Product>();

        foreach (var productId in createCategory.Products)
        {
            var product = await this.Products.GetOne(productId);

            if (product == null)
            {
                throw new CategoryServiceException("The provided product id is not valid.");
            }

            categoryProducts.Add(new(product.Id));
            products.Add(product);
        }

        var category = new Category(createCategory.Name, createCategory.Description, categoryProducts);

        await this.Categories.Save(category);

        foreach (var product in products)
        {
            await this.Products.AddOrUpdate(product);
        }

        return category;
    }

    public async Task<Category?> UpdateCategory(long categoryId, RequestModels.Category updateCategory)
    {
        var category = await this.Categories.GetOne(categoryId);
        if (category == null)
        {
            return null;
        }

        var productsToCheck = updateCategory.Products;
        var oldProducts = await this.Products.GetByCategoryId(categoryId);

        foreach (var oldProduct in oldProducts)
        {
            if (!productsToCheck.Contains(oldProduct.Id))
            {
                await this.Products.AddOrUpdate(oldProduct);

                category.RemoveProduct(new ProductId(oldProduct.Id));
            }

            productsToCheck.Remove(oldProduct.Id);
        }

        foreach (var updateId in productsToCheck)
        {
            var product = await this.Products.GetOne(updateId);

            if (product == null)
            {
                throw new CategoryServiceException("The provided product id is not valid.");
            }

            category.AddProduct(new ProductId(product.Id));

            product.SetCategory(new ProductCategory(category.Id, category.Name, category.Description));
            await this.Products.AddOrUpdate(product);
        }

        category.Rename(updateCategory.Name, updateCategory.Description);

        await this.Categories.Save(category);

        return category;
    }

    public async Task<Category?> RetireCategory(long categoryId)
    {
        var category = await this.Categories.GetOne(categoryId);

        if (category == null)
        {
            return null;
        }

        try
        {
            category.RetireCategory();

            await this.Categories.Save(category);

            return category;
        }
        catch (CategoryLifecycleException ex)
        {
            throw new CategoryServiceException(ex.Message, ex);
        }
    }
}

[Serializable]
public class CategoryServiceException : Exception
{
    public CategoryServiceException()
    {
    }

    public CategoryServiceException(string message)
        : base(message)
    {
    }

    public CategoryServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected CategoryServiceException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
