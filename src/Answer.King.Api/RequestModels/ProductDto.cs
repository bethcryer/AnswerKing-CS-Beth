namespace Answer.King.Api.RequestModels;

public record ProductDto
{
    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;

    public double Price { get; init; }

    public List<CategoryId> Categories { get; init; } = new List<CategoryId>();
}
