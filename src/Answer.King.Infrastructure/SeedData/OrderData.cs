using System;
using System.Collections.Generic;
using System.Linq;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using Answer.King.Infrastructure.Repositories.Mappings;

namespace Answer.King.Infrastructure.SeedData;

internal static class OrderData
{
    private static readonly OrderFactory OrderFactory = new();

    public static IList<Order> Orders { get; } = GetOrders();

    private static IList<Order> GetOrders()
    {
        return new List<Order>
        {
            new Order(),
            OrderWithLineItems(),
            CancelledOrder(),
        };
    }

    private static Order OrderWithLineItems()
    {
        var fish = ProductData.Products.SingleOrDefault(p => p.Id == 1);

        var fishOrder = new Product(fish!.Id, fish.Name, fish.Description, fish.Price);

        var lineItem1 = new LineItem(fishOrder);
        lineItem1.AddQuantity(1);

        var chips = ProductData.Products.SingleOrDefault(p => p.Id == 2);

        var chipsOrder = new Product(chips!.Id, chips.Name, chips.Description, chips.Price);

        var lineItem2 = new LineItem(chipsOrder);
        lineItem2.AddQuantity(2);

        var lineItems = new List<LineItem>
        {
            lineItem1,
            lineItem2,
        };

        return OrderFactory.CreateOrder(
            0,
            DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddMinutes(-10),
            OrderStatus.Created,
            lineItems);
    }

    private static Order CancelledOrder()
    {
        var lineItems = new List<LineItem>();

        return OrderFactory.CreateOrder(
            0,
            DateTime.UtcNow.AddHours(-3),
            DateTime.UtcNow.AddMinutes(-50),
            OrderStatus.Cancelled,
            lineItems);
    }
}
