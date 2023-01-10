using System.Runtime.Serialization;

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
        this._categories = new HashSet<CategoryId>();
        this._tags = new HashSet<TagId>();
    }

#pragma warning disable IDE0051 // Remove unused private members
    private Product(long id,
#pragma warning restore IDE0051 // Remove unused private members
        string name,
        string description,
        double price,
        IList<CategoryId> categories,
        IList<TagId> tags,
        bool retired)
    {
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);
        Guard.AgainstNullArgument(nameof(categories), categories);
        Guard.AgainstNullArgument(nameof(tags), tags);

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this._categories = new HashSet<CategoryId>(categories);
        this._tags = new HashSet<TagId>(tags);
        this.Retired = retired;
    }

    public long Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    private HashSet<CategoryId> _categories { get; }

    public IReadOnlyCollection<CategoryId> Categories => this._categories;

    private HashSet<TagId> _tags { get; }

    public IReadOnlyCollection<TagId> Tags => this._tags;

    public bool Retired { get; private set; }

    public void AddCategory(CategoryId category)
    {
        if (this.Retired)
        {
            throw new ProductLifecycleException("Cannot add category to retired product.");
        }

        this._categories.Add(category);
    }

    public void RemoveCategory(CategoryId category)
    {
        if (this.Retired)
        {
            throw new ProductLifecycleException("Cannot remove category from retired product.");
        }

        this._categories.Remove(category);
    }

    public void AddTag(TagId tag)
    {
        if (this.Retired)
        {
            throw new ProductLifecycleException("Cannot add tag to retired product.");
        }

        this._tags.Add(tag);
    }

    public void RemoveTag(TagId tag)
    {
        if (this.Retired)
        {
            throw new ProductLifecycleException("Cannot remove tag from retired product.");
        }

        this._tags.Remove(tag);
    }

    public void Retire()
    {
        this.Retired = true;
    }
}

[Serializable]
public class ProductLifecycleException : Exception
{
    public ProductLifecycleException()
    {
    }

    public ProductLifecycleException(string message)
        : base(message)
    {
    }

    public ProductLifecycleException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected ProductLifecycleException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
