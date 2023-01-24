using Answer.King.Domain;

namespace Answer.King.Api.RequestModels;

public record CategoryId
{
    public CategoryId(long value)
    {
        Guard.AgainstDefaultValue(nameof(value), value);
        this.Value = value;
    }

    public long Value { get; init; }

    public static implicit operator long(CategoryId id) => id.Value;
}
