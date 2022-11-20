using Answer.King.Domain.Orders;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;
using Payment = Answer.King.Domain.Repositories.Models.Payment;

namespace Answer.King.Api.Services;

public class PaymentService : IPaymentService
{
    public PaymentService(
        IPaymentRepository payments,
        IOrderRepository orders)
    {
        this.Payments = payments;
        this.Orders = orders;
    }

    private IPaymentRepository Payments { get; }

    private IOrderRepository Orders { get; }

    public async Task<IEnumerable<Payment>> GetPayments()
    {
        return await this.Payments.Get();
    }

    public async Task<Payment?> GetPayment(long paymentId)
    {
        return await this.Payments.Get(paymentId);
    }

    public async Task<Payment> MakePayment(Api.RequestModels.Payment makePayment)
    {
        var order = await this.Orders.Get(makePayment.OrderId);

        if (order == null)
        {
            throw new PaymentServiceException($"No order found for given order id: {makePayment.OrderId}.");
        }

        try
        {
            var payment = new Payment(order.Id, makePayment.Amount, order.OrderTotal);

            order.CompleteOrder();

            await this.Orders.Save(order);
            await this.Payments.Add(payment);

            return payment;
        }
        catch (PaymentException ex)
        {
            throw new PaymentServiceException(ex.Message, ex);
        }
        catch (OrderLifeCycleException ex)
        {
            var msg = ex.Message.Contains("paid", StringComparison.OrdinalIgnoreCase)
                ? "Cannot make payment as order has already been paid."
                : "Cannot make payment as order is cancelled.";

            throw new PaymentServiceException(msg, ex);
        }
    }
}

[Serializable]
internal class PaymentServiceException : Exception
{
    public PaymentServiceException(string message) : base(message)
    {
    }

    public PaymentServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public PaymentServiceException() : base()
    {
    }
}
