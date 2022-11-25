using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Infrastructure.SeedData;

public class ProductDataSeeder : ISeedData
{
    public void SeedData(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();
        var collection = db.GetCollection<Product>();

        if (this.DataSeeded)
        {
            return;
        }

        var none = collection.Count() < 1;
        if (none)
        {
            collection.InsertBulk(ProductData.Products);
        }

        this.DataSeeded = true;
    }

    private bool DataSeeded { get; set; }
}
