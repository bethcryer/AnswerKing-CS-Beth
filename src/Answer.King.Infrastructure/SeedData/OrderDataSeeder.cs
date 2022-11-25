using Answer.King.Domain.Orders;

namespace Answer.King.Infrastructure.SeedData;

public class OrderDataSeeder : ISeedData
{
    public void SeedData(ILiteDbConnectionFactory connections)
    {
        var db = connections.GetConnection();
        var collection = db.GetCollection<Order>();

        if (this.DataSeeded)
        {
            return;
        }

        var none = collection.Count() < 1;
        if (none)
        {
            collection.InsertBulk(OrderData.Orders);
        }

        this.DataSeeded = true;
    }

    private bool DataSeeded { get; set; }
}
