using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Domain.Orders.Models;

public class Product
{
    public Product(long id, string name, string description, double price, IList<Category> categories)
    {
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);
        Guard.AgainstNullArgument(nameof(categories), categories);

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this._Categories = categories;
    }

    public long Id { get; }

    public string Name { get; }

    public string Description { get; }

    public double Price { get; }

    private IList<Category> _Categories { get; }

    public IReadOnlyCollection<Category> Categories => (this._Categories as List<Category>)!;
}
