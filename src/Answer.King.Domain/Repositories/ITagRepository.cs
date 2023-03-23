using Answer.King.Domain.Inventory;

namespace Answer.King.Domain.Repositories;

public interface ITagRepository : IAggregateRepository<Tag>, IBaseRepository
{
    Task<Tag?> GetOne(string name);
}
