namespace Answer.King.Api.IntegrationTests.Common.Models;

public class Product
{
    public Product(long id, string name, string description, double price, Domain.Repositories.Models.ProductCategory category, bool retired)
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.Category = category;
        this.Retired = retired;
    }

    public long Id { get; }

    public string Name { get; }

    public string Description { get; }

    public double Price { get; }

    public Domain.Repositories.Models.ProductCategory Category { get; }

    public bool Retired { get; }
}
