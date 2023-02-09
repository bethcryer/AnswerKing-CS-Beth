using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetOne(long id);

    Task<Product?> GetOne(string name);

    Task<IEnumerable<Product>> GetAll();

    Task<IEnumerable<Product>> GetMany(IEnumerable<long> ids);

    Task AddOrUpdate(Product product);

    Task<IEnumerable<Product>> GetByCategoryId(long categoryId);

    Task<IEnumerable<Product>> GetByCategoryId(params long[] categoryIds);
}
