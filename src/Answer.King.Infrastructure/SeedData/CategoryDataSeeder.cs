using Answer.King.Domain.Inventory;

namespace Answer.King.Infrastructure.SeedData;

public class CategoryDataSeeder : ISeedData
{
    private bool DataSeeded { get; set; }

    public void SeedData(ILiteDbConnectionFactory connections)
    {
        if (this.DataSeeded)
        {
            return;
        }

        var db = connections.GetConnection();
        var collection = db.GetCollection<Category>();

        var none = collection.Count() < 1;
        if (none)
        {
            collection.InsertBulk(CategoryData.Categories);
        }

        this.DataSeeded = true;
    }
}
