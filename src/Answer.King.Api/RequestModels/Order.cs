namespace Answer.King.Api.RequestModels;

public record Order
{
    public IList<LineItem> LineItems { get; init; } = null!;
}
