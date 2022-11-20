using Answer.King.Api.RequestModels;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Inventory;
using Category = Answer.King.Domain.Inventory.Category;

namespace Answer.King.Api.Services;

public class CategoryService : ICategoryService
{
    public CategoryService(ICategoryRepository categories)
    {
        this.Categories = categories;
    }

    private ICategoryRepository Categories { get; }

    public async Task<Category?> GetCategory(long categoryId)
    {
        return await this.Categories.Get(categoryId);
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await this.Categories.Get();
    }

    public async Task<Category> CreateCategory(RequestModels.Category createCategory)
    {
        var category = new Category(createCategory.Name, createCategory.Description);

        await this.Categories.Save(category);

        return category;
    }

    public async Task<Category?> UpdateCategory(long categoryId, RequestModels.Category updateCategory)
    {
        var category = await this.Categories.Get(categoryId);
        if (category == null)
        {
            return null;
        }

        category.Rename(updateCategory.Name, updateCategory.Description);

        await this.Categories.Save(category);

        return category;
    }

    public async Task<Category?> RetireCategory(long categoryId)
    {
        var category = await this.Categories.Get(categoryId);

        if (category == null)
        {
            return null;
        }

        if (category.Retired)
        {
            throw new CategoryServiceException("The category is already retired.");
        }

        try
        {
            category.RetireCategory();

            await this.Categories.Save(category);

            return category;
        }
        catch (CategoryLifecycleException ex)
        {
            // ignored
            throw new CategoryServiceException(
                $"Cannot retire category whilst there are still products assigned. {string.Join(',', category.Products.Select(p => p.Value))}"
                , ex);
        }
    }
}

[Serializable]
internal class CategoryServiceException : Exception
{
    public CategoryServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public CategoryServiceException() : base()
    {
    }

    public CategoryServiceException(string? message) : base(message)
    {
    }
}
