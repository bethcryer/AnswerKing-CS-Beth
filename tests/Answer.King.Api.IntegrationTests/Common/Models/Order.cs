using Answer.King.Domain;

namespace Answer.King.Api.IntegrationTests.Common.Models;

public class Order
{
    public Order(
        long id,
        string createdOn,
        string lastUpdated,
        string orderStatus,
        double orderTotal,
        IList<LineItem>? lineItems)
    {
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(createdOn), createdOn);
        Guard.AgainstNullOrEmptyArgument(nameof(lastUpdated), lastUpdated);
        Guard.AgainstNullOrEmptyArgument(nameof(orderStatus), orderStatus);
        Guard.AgainstNegativeValue(nameof(orderTotal), orderTotal);

        this.Id = id;
        this.CreatedOn = createdOn;
        this.LastUpdated = lastUpdated;
        this.OrderStatus = orderStatus;
        this.OrderTotal = orderTotal;
        this.LineItems = lineItems ?? new List<LineItem>();
    }

    public long Id { get; set; }

    public string CreatedOn { get; set; }

    public string LastUpdated { get; set; }

    public string OrderStatus { get; set; }

    public double OrderTotal { get; set; }

    public IList<LineItem> LineItems { get; }
}

public class LineItem
{
    public LineItem(OrderProduct product, long quantity, double subTotal)
    {
        Guard.AgainstDefaultValue(nameof(quantity), quantity);
        Guard.AgainstNegativeValue(nameof(subTotal), subTotal);

        this.Product = product;
        this.Quantity = quantity;
        this.SubTotal = subTotal;
    }
    public OrderProduct Product { get; set; }
    public long Quantity { get; set; }
    public double SubTotal { get; set; }
}

public class OrderProduct
{
    public OrderProduct(long id, string name, string description, double price)
    {
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
    }
    public long Id { get; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }
}
