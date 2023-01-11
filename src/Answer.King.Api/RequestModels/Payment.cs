namespace Answer.King.Api.RequestModels;

public record Payment
{
    public double Amount { get; init; }

    public long OrderId { get; init; }
}
