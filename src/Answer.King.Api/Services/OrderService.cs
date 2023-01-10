using Answer.King.Api.Services.Extensions;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Repositories;

namespace Answer.King.Api.Services;

using System.Runtime.Serialization;

public class OrderService : IOrderService
{
    public OrderService(
        IOrderRepository orders,
        IProductRepository products)
    {
        this.Orders = orders;
        this.Products = products;
    }

    private IOrderRepository Orders { get; }

    private IProductRepository Products { get; }

    public async Task<Order?> GetOrder(long orderId)
    {
        return await this.Orders.GetOne(orderId);
    }

    public async Task<IEnumerable<Order>> GetOrders()
    {
        return await this.Orders.GetAll();
    }

    public async Task<Order> CreateOrder(RequestModels.Order createOrder)
    {
        var submittedProductIds = createOrder.LineItems.Select(l => l.ProductId).ToList();

        var matchingProducts =
            (await this.Products.GetMany(submittedProductIds)).ToList();

        var invalidProducts =
            submittedProductIds.Except(matchingProducts.Select(p => p.Id))
                .ToList();

        if (invalidProducts.Any())
        {
            throw new ProductInvalidException(
                $"Product id{(invalidProducts.Count > 1 ? "s" : string.Empty)} does not exist: {string.Join(',', invalidProducts)}");
        }

        var order = new Order();
        order.AddOrRemoveLineItems(createOrder, matchingProducts);

        await this.Orders.Save(order);

        return order;
    }

    public async Task<Order?> UpdateOrder(long orderId, RequestModels.Order updateOrder)
    {
        var order = await this.Orders.GetOne(orderId);

        if (order == null)
        {
            return null;
        }

        var submittedProductIds = updateOrder.LineItems.Select(l => l.ProductId).ToList();

        var matchingProducts =
            (await this.Products.GetMany(submittedProductIds)).ToList();

        var invalidProducts =
            submittedProductIds.Except(matchingProducts.Select(p => p.Id))
                .ToList();

        if (invalidProducts.Count > 0)
        {
            throw new ProductInvalidException(
                $"Product id{(invalidProducts.Count > 1 ? "s" : string.Empty)} does not exist: {string.Join(',', invalidProducts)}");
        }

        order.AddOrRemoveLineItems(updateOrder, matchingProducts);

        await this.Orders.Save(order);

        return order;
    }

    public async Task<Order?> CancelOrder(long orderId)
    {
        var order = await this.Orders.GetOne(orderId);

        if (order == null)
        {
            return null;
        }

        order.CancelOrder();
        await this.Orders.Save(order);

        return order;
    }
}

[Serializable]
public class ProductInvalidException : Exception
{
    public ProductInvalidException(string message)
        : base(message)
    {
    }

    public ProductInvalidException()
    {
    }

    public ProductInvalidException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected ProductInvalidException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base(serializationInfo, streamingContext)
    {
    }
}
