namespace Answer.King.Domain.Repositories.Models;

public record CategoryId
{
    public CategoryId(long value)
    {
        Guard.AgainstDefaultValue(nameof(value), value);

        this.Value = value;
    }

    public long Value { get; }

    public static implicit operator long(CategoryId id) => id.Value;
}
