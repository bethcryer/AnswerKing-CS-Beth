using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Repositories;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    public TagRepository(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();

        this.Collection = db.GetCollection<Tag>();
        this.Collection.EnsureIndex("products");
    }

    private ILiteCollection<Tag> Collection { get; }

    public Task<IEnumerable<Tag>> Get()
    {
        return Task.FromResult(this.Collection.FindAll());
    }

    public Task<Tag?> Get(long id)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Id == id))!;
    }

    public Task Save(Tag item)
    {
        return Task.FromResult(this.Collection.Upsert(item));
    }
}
