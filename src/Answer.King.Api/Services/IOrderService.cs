using Answer.King.Domain.Orders;

namespace Answer.King.Api.Services;

public interface IOrderService
{
    Task<Order> CreateOrder(RequestModels.Order createOrder);

    Task<Order?> GetOrder(long orderId);

    Task<IEnumerable<Order>> GetOrders();

    Task<Order?> UpdateOrder(long orderId, RequestModels.Order updateOrder);

    Task<Order?> CancelOrder(long orderId);
}