namespace Answer.King.Domain.Repositories.Models;

public class Product
{
    public Product(string name, string description, double price)
    {
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);

        this.Id = 0;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this._Categories = new HashSet<CategoryId>();
    }

    private Product(long id,
        string name,
        string description,
        double price,
        IList<CategoryId> categories,
        bool retired)
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
        this._Categories = new HashSet<CategoryId>(categories);
        this.Retired = retired;
    }

    public long Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    private HashSet<CategoryId> _Categories { get; }

    public IReadOnlyCollection<CategoryId> Categories => this._Categories;

    public bool Retired { get; private set; }

    public void AddCategory(CategoryId category)
    {
        if (this.Retired)
        {
            throw new ProductLifecycleException("Cannot add category to retired product.");
        }

        this._Categories.Add(category);
    }

    public void RemoveCategory(CategoryId category)
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
