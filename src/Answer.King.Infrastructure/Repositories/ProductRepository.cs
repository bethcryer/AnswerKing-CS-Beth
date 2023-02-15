using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace Answer.King.Infrastructure.Repositories;

public class ProductRepository : BaseRepository, IProductRepository
{
    private readonly ILogger<ProductRepository> logger;

    public ProductRepository(ILiteDbConnectionFactory connections, ILogger<ProductRepository> logger)
        : base(connections)
    {
        this.Collection = this.Db.GetCollection<Product>();
        this.Collection.EnsureIndex("categories");

        this.logger = logger;
    }

    private ILiteCollection<Product> Collection { get; }

    public Task<Product?> GetOne(long id)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Id == id))!;
    }

    public Task<Product?> GetOne(string name)
    {
        return Task.FromResult(this.Collection.FindOne(c => c.Name == name))!;
    }

    public Task<IEnumerable<Product>> GetAll()
    {
        this.logger.LogInformation("Get all Products repository call");
        return Task.FromResult(this.Collection.FindAll());
    }

    public Task<IEnumerable<Product>> GetMany(IEnumerable<long> ids)
    {
        return Task.FromResult(this.Collection.Find(p => ids.Contains(p.Id)));
    }

    public Task AddOrUpdate(Product product)
    {
        return Task.FromResult(this.Collection.Upsert(product));
    }

    public Task<IEnumerable<Product>> GetByCategoryId(long categoryId)
    {
        return Task.FromResult(this.Collection.Find(p => p.Category.Id.Equals(categoryId)));
    }

    public Task<IEnumerable<Product>> GetByCategoryId(params long[] categoryIds)
    {
        var query = Query.In("categories[*] ANY", categoryIds.Select(c => new BsonValue(c)));
        return Task.FromResult(this.Collection.Find(query));
    }
}
