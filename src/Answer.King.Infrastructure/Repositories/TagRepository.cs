using System.Collections.Generic;
using System.Threading.Tasks;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Repositories;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories;

public class TagRepository : BaseRepository, ITagRepository
{
    public TagRepository(ILiteDbConnectionFactory connections)
        : base(connections)
    {
        this.Collection = this.Db.GetCollection<Tag>();
        this.Collection.EnsureIndex("products");
    }

    private ILiteCollection<Tag> Collection { get; }

    public Task<IEnumerable<Tag>> GetAll()
    {
        return Task.FromResult(this.Collection.FindAll());
    }

    public Task<Tag?> GetOne(long id)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Id == id))!;
    }

    public Task<Tag?> GetOne(string name)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Name == name))!;
    }

    public Task Save(Tag item)
    {
        return Task.FromResult(this.Collection.Upsert(item));
    }
}
