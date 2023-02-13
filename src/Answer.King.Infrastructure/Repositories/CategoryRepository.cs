using System.Collections.Generic;
using System.Linq;
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
        this.Collection.EnsureIndex("products");
    }

    private ILiteCollection<Category> Collection { get; }

    public Task<IEnumerable<Category>> GetAll()
    {
        return Task.FromResult(this.Collection.FindAll());
    }

    public Task<Category?> GetOne(long id)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Id == id))!;
    }

    public Task<Category?> GetOne(string name)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Name == name))!;
    }

    public Task Save(Category item)
    {
        return Task.FromResult(this.Collection.Upsert(item));
    }

    public Task<IEnumerable<Category>> GetByProductId(long productId)
    {
        var query = Query.EQ("products[*] ANY", productId);
        return Task.FromResult(this.Collection.Find(query));
    }

    public Task<IEnumerable<Category>> GetByProductId(params long[] productIds)
    {
        var query = Query.In("products[*] ANY", productIds.Select(c => new BsonValue(c)));
        return Task.FromResult(this.Collection.Find(query));
    }
}
