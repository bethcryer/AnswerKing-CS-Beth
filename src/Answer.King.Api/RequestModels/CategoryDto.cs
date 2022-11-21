namespace Answer.King.Api.RequestModels;

public record CategoryDto
{
    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;

    public List<ProductId> Products { get; init; } = null!;
}
