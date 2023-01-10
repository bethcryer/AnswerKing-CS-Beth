namespace Answer.King.Api.RequestModels;

public record Tag
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
}
