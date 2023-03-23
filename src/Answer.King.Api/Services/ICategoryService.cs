using Answer.King.Domain.Inventory;

namespace Answer.King.Api.Services;

public interface ICategoryService
{
    Task<Category> CreateCategory(RequestModels.Category createCategory);

    Task<IEnumerable<Category>> GetCategories();

    Task<Category?> GetCategory(long categoryId);

    Task<Category?> GetCategoryByName(string name);

    Task<Category?> RetireCategory(long categoryId);

    Task<Category?> UnretireCategory(long categoryId);

    Task<Category?> UpdateCategory(long categoryId, RequestModels.Category updateCategory);
}
