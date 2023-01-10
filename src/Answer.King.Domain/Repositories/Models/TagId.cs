namespace Answer.King.Domain.Repositories.Models;

public record TagId
{
    public TagId(long value)
    {
        Guard.AgainstDefaultValue(nameof(value), value);

        this.Value = value;
    }

    public long Value { get; }

    public static implicit operator long(TagId id) => id.Value;
}
