using System.Runtime.Serialization;
using Answer.King.Domain.Orders.Models;

namespace Answer.King.Domain.Orders;

public class Order : IAggregateRoot
{
    private readonly IList<LineItem> lineItems;

    public Order()
    {
        this.Id = 0;
        this.LastUpdated = this.CreatedOn = DateTime.UtcNow;
        this.OrderStatus = OrderStatus.Created;
        this.lineItems = new List<LineItem>();
    }

    // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
    private Order(
        long id,
        DateTime createdOn,
        DateTime lastUpdated,
        OrderStatus status,
        IList<LineItem>? lineItems)
    {
#pragma warning restore IDE0051 // Remove unused private members
        Guard.AgainstDefaultValue(nameof(createdOn), createdOn);
        Guard.AgainstDefaultValue(nameof(lastUpdated), lastUpdated);

        this.Id = id;
        this.CreatedOn = createdOn;
        this.LastUpdated = lastUpdated;
        this.OrderStatus = status;
        this.lineItems = lineItems ?? new List<LineItem>();
    }

    public long Id { get; }

    public DateTime CreatedOn { get; }

    public DateTime LastUpdated { get; private set; }

    public OrderStatus OrderStatus { get; private set; }

    public double OrderTotal => this.LineItems.Sum(li => li.SubTotal);

    public IReadOnlyCollection<LineItem> LineItems => (this.lineItems as List<LineItem>)!;

    public void AddLineItem(long productId, string productName, string productDescription, double price, int quantity = 1)
    {
        Guard.AgainstDefaultValue(nameof(productId), productId);

        if (this.OrderStatus != OrderStatus.Created)
        {
            throw new OrderLifeCycleException($"Cannot add line item - Order status {this.OrderStatus}.");
        }

        var lineItem = this.lineItems.SingleOrDefault(li => li.Product.Id == productId);

        if (lineItem == null)
        {
            var product = new Product(productId, productName, productDescription, price);
            lineItem = new LineItem(product);
            this.lineItems.Add(lineItem);
        }

        lineItem.AddQuantity(quantity);
        this.LastUpdated = DateTime.UtcNow;
    }

    public void RemoveLineItem(long productId, int quantity = 1)
    {
        Guard.AgainstDefaultValue(nameof(productId), productId);

        if (this.OrderStatus != OrderStatus.Created)
        {
            throw new OrderLifeCycleException($"Cannot remove line item - Order status {this.OrderStatus}.");
        }

        var lineItem = this.lineItems.SingleOrDefault(li => li.Product.Id == productId);

        if (lineItem == null)
        {
            return;
        }

        lineItem.RemoveQuantity(quantity);

        if (lineItem.Quantity <= 0)
        {
            this.lineItems.Remove(lineItem);
        }

        this.LastUpdated = DateTime.UtcNow;
    }

    public void CompleteOrder()
    {
        if (this.OrderStatus != OrderStatus.Created)
        {
            throw new OrderLifeCycleException($"Cannot complete order - Order status {this.OrderStatus}.");
        }

        this.OrderStatus = OrderStatus.Complete;
        this.LastUpdated = DateTime.UtcNow;
    }

    public void CancelOrder()
    {
        if (this.OrderStatus != OrderStatus.Created)
        {
            throw new OrderLifeCycleException($"Cannot cancel order - Order status {this.OrderStatus}.");
        }

        this.OrderStatus = OrderStatus.Cancelled;
        this.LastUpdated = DateTime.UtcNow;
    }
}

[Serializable]
public class OrderPaymentException : Exception
{
    public OrderPaymentException(string message)
        : base(message)
    {
    }

    public OrderPaymentException()
    {
    }

    public OrderPaymentException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected OrderPaymentException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

[Serializable]
public class OrderLifeCycleException : Exception
{
    public OrderLifeCycleException(string message)
        : base(message)
    {
    }

    public OrderLifeCycleException()
    {
    }

    public OrderLifeCycleException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected OrderLifeCycleException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
