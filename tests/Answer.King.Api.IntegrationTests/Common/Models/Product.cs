namespace Answer.King.Api.IntegrationTests.Common.Models;

public class Product
{
    public Product(long id, string name, string description, double price, IList<long>? categories, bool retired)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.Categories = categories ?? new List<long>();
        this.Retired = retired;
    }

    public long Id { get; }

    public string Name { get; }

    public string Description { get; }

    public double Price { get; }

    public IList<long> Categories { get; }

    public bool Retired { get; }
}
