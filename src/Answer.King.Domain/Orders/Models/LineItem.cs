using System.Runtime.Serialization;

namespace Answer.King.Domain.Orders.Models;

public class LineItem
{
    public LineItem(Product product)
    {
        Guard.AgainstNullArgument(nameof(product), product);

        this.Product = product;
    }

    public Product Product { get; }

    public int Quantity { get; private set; }

    public double SubTotal => this.Product.Price * this.Quantity;

    public void AddQuantity(int quantity)
    {
        if (quantity < 1)
        {
            throw new LineItemException("Cannot add less than 1 to the quantity.");
        }

        this.Quantity += quantity;
    }

    public void RemoveQuantity(int quantity)
    {
        if (quantity < 1)
        {
            throw new LineItemException("Cannot remove less than 1 from the quantity.");
        }

        if (this.Quantity < quantity)
        {
            throw new LineItemException($"Cannot remove {quantity} from {this.Quantity}.");
        }

        this.Quantity -= quantity;
    }
}

[Serializable]
public class LineItemException : Exception
{
    public LineItemException(string message)
        : base(message)
    {
    }

    public LineItemException()
    {
    }

    public LineItemException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected LineItemException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
