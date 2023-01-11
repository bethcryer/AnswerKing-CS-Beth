namespace Answer.King.Domain.Inventory.Models;

public record ProductId
{
    public ProductId(long value)
    {
        Guard.AgainstDefaultValue(nameof(value), value);

        this.Value = value;
    }

    public long Value { get; }

    public static implicit operator long(ProductId id) => id.Value;
}
