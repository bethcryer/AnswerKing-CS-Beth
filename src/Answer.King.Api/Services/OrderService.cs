using Answer.King.Api.Services.Extensions;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using Answer.King.Domain.Repositories;


namespace Answer.King.Api.Services;

public class OrderService : IOrderService
{
    public OrderService(
        IOrderRepository orders,
        IProductRepository products,
        ICategoryRepository categories)
    {
        this.Orders = orders;
        this.Products = products;
        this.Categories = categories;
    }

    private IOrderRepository Orders { get; }

    private IProductRepository Products { get; }

    private ICategoryRepository Categories { get; }

    public async Task<Order?> GetOrder(long orderId)
    {
        return await this.Orders.Get(orderId);
    }

    public async Task<IEnumerable<Order>> GetOrders()
    {
        return await this.Orders.Get();
    }

    public async Task<Order> CreateOrder(RequestModels.Order createOrder)
    {
        var submittedProductIds = createOrder.LineItems.Select(l => l.ProductId).ToList();

        var matchingProducts =
            (await this.Products.Get(submittedProductIds)).ToList();

        var invalidProducts =
            submittedProductIds.Except(matchingProducts.Select(p => p.Id))
                .ToList();

        if (invalidProducts.Any())
        {
            throw new ProductInvalidException(
                $"Product id{(invalidProducts.Count > 1 ? "s" : "")} does not exist: {string.Join(',', invalidProducts)}");
        }

        var categories =
            (await this.Categories.GetByProductId(matchingProducts.Select(p => p.Id).ToArray()))
                .Select(c => new Category(c.Id, c.Name, c.Description)).ToList();

        var order = new Order();
        order.AddOrRemoveLineItems(createOrder, matchingProducts, categories);

        await this.Orders.Save(order);

        return order;
    }

    public async Task<Order?> UpdateOrder(long orderId, RequestModels.Order updateOrder)
    {
        var order = await this.Orders.Get(orderId);

        if (order == null)
        {
            return null;
        }

        var submittedProductIds = updateOrder.LineItems.Select(l => l.ProductId).ToList();

        var matchingProducts =
            (await this.Products.Get(submittedProductIds)).ToList();

        var invalidProducts =
            submittedProductIds.Except(matchingProducts.Select(p => p.Id))
                .ToList();

        if (invalidProducts.Count > 0)
        {
            throw new ProductInvalidException(
                $"Product id{(invalidProducts.Count > 1 ? "s" : "")} does not exist: {string.Join(',', invalidProducts)}");
        }

        var categories =
            (await this.Categories.GetByProductId(matchingProducts.Select(p => p.Id).ToArray()))
            .Select(c => new Category(c.Id, c.Name, c.Description)).ToList();

        order.AddOrRemoveLineItems(updateOrder, matchingProducts, categories);

        await this.Orders.Save(order);

        return order;
    }

    public async Task<Order?> CancelOrder(long orderId)
    {
        var order = await this.Orders.Get(orderId);

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
internal class ProductInvalidException : Exception
{
    public ProductInvalidException(string message) : base(message)
    {
    }

    public ProductInvalidException() : base()
    {
    }

    public ProductInvalidException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
