using Answer.King.Domain.Orders;

namespace Answer.King.Domain.Repositories;

public interface IOrderRepository : IAggregateRepository<Order>, IBaseRepository
{
}
