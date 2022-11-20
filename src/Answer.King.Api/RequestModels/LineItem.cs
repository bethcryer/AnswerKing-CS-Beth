namespace Answer.King.Api.RequestModels;

public record LineItem
{
    public long ProductId { get; init; }

    public int Quantity { get; init; }
}
