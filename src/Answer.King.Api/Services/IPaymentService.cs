using Payment = Answer.King.Api.RequestModels.Payment;

namespace Answer.King.Api.Services;

public interface IPaymentService
{
    Task<Domain.Repositories.Models.Payment?> GetPayment(long paymentId);
    Task<IEnumerable<Domain.Repositories.Models.Payment>> GetPayments();
    Task<Domain.Repositories.Models.Payment> MakePayment(Payment makePayment);
}
