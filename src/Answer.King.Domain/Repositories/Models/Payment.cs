namespace Answer.King.Domain.Repositories.Models;

public class Payment
{
    public Payment(long orderId, double amount, double orderTotal)
    {
        Domain.Guard.AgainstNegativeValue(nameof(amount), amount);
        Domain.Guard.AgainstNegativeValue(nameof(orderTotal), orderTotal);
        Guard.AgainstUnderPayment(amount, orderTotal);

        this.Id = 0;
        this.OrderId = orderId;
        this.Amount = amount;
        this.OrderTotal = orderTotal;
        this.Date = DateTime.UtcNow;
    }

#pragma warning disable IDE0051 // Remove unused private members
    private Payment(long id, long orderId, double amount, double orderTotal, DateTime date)
#pragma warning restore IDE0051 // Remove unused private members
    {
        Domain.Guard.AgainstDefaultValue(nameof(id), id);
        Domain.Guard.AgainstDefaultValue(nameof(orderId), orderId);
        Domain.Guard.AgainstNegativeValue(nameof(amount), amount);
        Domain.Guard.AgainstNegativeValue(nameof(orderTotal), orderTotal);
        Domain.Guard.AgainstDefaultValue(nameof(date), date);
        Guard.AgainstUnderPayment(amount, orderTotal);

        this.Id = id;
        this.OrderId = orderId;
        this.Amount = amount;
        this.OrderTotal = orderTotal;
        this.Date = date;
    }

    public long Id { get; }

    public long OrderId { get; }

    public double Amount { get; }

    public double OrderTotal { get; }

    public double Change => this.Amount - this.OrderTotal;

    public DateTime Date { get; }

    public static class Guard
    {
        public static void AgainstUnderPayment(double paymentAmount, double orderTotal)
        {
            if (paymentAmount < orderTotal)
            {
                throw new PaymentException($"Payment amount '{paymentAmount}' does not cover Order Total '{orderTotal}'.");
            }
        }
    }
}
