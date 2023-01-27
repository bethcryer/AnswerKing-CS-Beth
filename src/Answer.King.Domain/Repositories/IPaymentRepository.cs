using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Domain.Repositories;

public interface IPaymentRepository
{
    Task<Payment> GetOne(long id);

    Task<IEnumerable<Payment>> GetAll();

    Task Add(Payment payment);
}
