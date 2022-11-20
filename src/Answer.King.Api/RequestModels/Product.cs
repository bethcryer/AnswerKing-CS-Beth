namespace Answer.King.Api.RequestModels;

public record Product
{
    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;

    public double Price { get; init; }

    public List<long> Categories { get; init; } = new List<long>();
}
