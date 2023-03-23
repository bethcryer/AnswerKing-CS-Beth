using System.Collections.Generic;
using System.Threading.Tasks;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories;

public class PaymentRepository : BaseRepository, IPaymentRepository
{
    public PaymentRepository(ILiteDbConnectionFactory connections)
        : base(connections)
    {
        this.Collection = this.Db.GetCollection<Payment>();
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
