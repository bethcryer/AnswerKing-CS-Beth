using System.Collections.Generic;
using System.Threading.Tasks;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Repositories;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    public CategoryRepository(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();

        this.Collection = db.GetCollection<Category>();
        this.Collection.EnsureIndex("Products._id");
    }

    private ILiteCollection<Category> Collection { get; }


    public Task<IEnumerable<Category>> Get()
    {
        return Task.FromResult(this.Collection.FindAll());
    }

    public Task<Category?> Get(long id)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Id == id))!;
    }

    public Task Save(Category item)
    {
        return Task.FromResult(this.Collection.Upsert(item));
    }

    public Task<IEnumerable<Category>> GetByProductId(long productId)
    {
        var query = Query.EQ("Products[*]._id ANY", productId);
        return Task.FromResult(this.Collection.Find(query))!;
    }
}
