namespace Answer.King.Domain.Orders.Models;

public class Product
{
    public Product(long id, string name, string description, double price)
    {
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
    }

    public long Id { get; }

    public string Name { get; }

    public string Description { get; }

    public double Price { get; }
}
