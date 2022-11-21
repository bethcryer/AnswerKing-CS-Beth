using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Orders.Models;

namespace Answer.King.Domain.Repositories.Models;

public class Product
{
    public Product(string name, string description, double price, IList<Category> categories)
    {
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);

        this.Id = 0;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this._Categories = new HashSet<Category>(categories ?? Array.Empty<Category>());
    }

    private Product(long id,
        string name,
        string description,
        double price,
        IList<Category>? categories,
        bool retired)
    {
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this._Categories = new HashSet<Category>(categories ?? Array.Empty<Category>());
        this.Retired = retired;
    }

    public long Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    private HashSet<Category> _Categories { get; }

    public IReadOnlyCollection<Category> Categories => this._Categories;

    public bool Retired { get; private set; }

    public void AddCategory(Category category)
    {
        if (this.Retired)
        {
            throw new ProductLifecycleException("Cannot add category to retired product.");
        }

        this._Categories.Add(category);
    }

    public void RemoveCategory(Category category)
    {
        if (this.Retired)
        {
            throw new ProductLifecycleException("Cannot remove category from retired product.");
        }

        this._Categories.Remove(category);
    }

    public void Retire()
    {
        this.Retired = true;
    }
}

[Serializable]
public class ProductLifecycleException : Exception
{
    public ProductLifecycleException(string message) : base(message)
    {
    }

    public ProductLifecycleException() : base()
    {
    }

    public ProductLifecycleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
