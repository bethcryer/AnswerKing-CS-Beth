using Answer.King.Domain.Inventory;

namespace Answer.King.Domain.Repositories;

public interface ICategoryRepository : IAggregateRepository<Category>
{
    Task<IEnumerable<Category>> GetByProductId(long productId);
}
