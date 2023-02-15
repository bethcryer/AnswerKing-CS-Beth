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

    public async Task<Category?> GetCategoryByName(string name)
    {
        return await this.Categories.GetOne(name);
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await this.Categories.GetAll();
    }

    public async Task<Category> CreateCategory(RequestModels.Category createCategory)
    {
        var products = new List<Product>();
        var category = new Category(createCategory.Name, createCategory.Description, new List<ProductId>());

        foreach (var productId in createCategory.Products)
        {
            var product = await this.Products.GetOne(productId) ??
                          throw new CategoryServiceException("The provided product id is not valid.");

            products.Add(product);

            await this.RemoveProductFromCategory(product);

            category.AddProduct(new ProductId(product.Id));
        }

        await this.Categories.Save(category);

        foreach (var product in products)
        {
            product.SetCategory(new ProductCategory(category.Id, category.Name, category.Description));

            await this.Products.AddOrUpdate(product);
        }

        return category;
    }

    public async Task<Category?> UpdateCategory(long categoryId, RequestModels.Category updateCategory)
    {
        var products = new List<Product>();
        var category = await this.Categories.GetOne(categoryId);
        if (category == null)
        {
            return null;
        }

        var categoryProducts = await this.Products.GetByCategoryId(categoryId);
        var newProducts = updateCategory.Products.Where(p => categoryProducts.All(p2 => p2.Id != p));

        foreach (var updateId in newProducts)
        {
            var product = await this.Products.GetOne(updateId) ??
                          throw new CategoryServiceException("The provided product id is not valid.");

            products.Add(product);

            await this.RemoveProductFromCategory(product);

            category.AddProduct(new ProductId(product.Id));
        }

        category.Rename(updateCategory.Name, updateCategory.Description);

        await this.Categories.Save(category);

        foreach (var product in products)
        {
            product.SetCategory(new ProductCategory(category.Id, category.Name, category.Description));

            await this.Products.AddOrUpdate(product);
        }

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

    private async Task RemoveProductFromCategory(Product product)
    {
        var oldCategory = await this.Categories.GetOne(product.Category.Id) ??
                          throw new CategoryServiceException("Failed to remove product from old category.");

        oldCategory.RemoveProduct(new ProductId(product.Id));
        await this.Categories.Save(oldCategory);
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
