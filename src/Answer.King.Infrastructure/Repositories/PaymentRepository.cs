using System.Collections.Generic;
using System.Threading.Tasks;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    public PaymentRepository(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();

        this.Collection = db.GetCollection<Payment>();
    }

    private ILiteCollection<Payment> Collection { get; }

    public Task<Payment> GetOne(long id)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Id == id));
    }

    public Task<IEnumerable<Payment>> GetAll()
    {
        return Task.FromResult(this.Collection.FindAll());
    }

    public Task Add(Payment payment)
    {
        return Task.FromResult(this.Collection.Insert(payment));
    }
}
