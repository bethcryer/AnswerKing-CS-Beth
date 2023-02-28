using System.Runtime.Serialization;

namespace Answer.King.Domain.Repositories.Models;

public class Product
{
    private readonly HashSet<TagId> tags;

    public Product(string name, string description, double price, ProductCategory category)
    {
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);

        this.Id = 0;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.LastUpdated = this.CreatedOn = DateTime.UtcNow;
        this.Category = category;
        this.tags = new HashSet<TagId>();
    }

    // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
    private Product(
        long id,
        string name,
        string description,
        double price,
        DateTime createdOn,
        DateTime lastUpdated,
        ProductCategory category,
        IList<TagId> tags,
        bool retired)
    {
#pragma warning restore IDE0051 // Remove unused private members
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNegativeValue(nameof(price), price);
        Guard.AgainstNullArgument(nameof(category), category);
        Guard.AgainstNullArgument(nameof(tags), tags);

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.Price = price;
        this.CreatedOn = createdOn;
        this.LastUpdated = lastUpdated;
        this.Category = category;
        this.tags = new HashSet<TagId>(tags);
        this.Retired = retired;
    }

    public long Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double Price { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime LastUpdated { get; set; }

    public ProductCategory Category { get; private set; }

    public IReadOnlyCollection<TagId> Tags => this.tags;

    public bool Retired { get; private set; }

    public void AddTag(TagId tag)
    {
        if (this.Retired && !this.Tags.Contains(tag))
        {
            throw new ProductLifecycleException("Cannot add tag to retired product.");
        }

        this.tags.Add(tag);

        this.LastUpdated = DateTime.UtcNow;
    }

    public void RemoveTag(TagId tag)
    {
        if (this.Retired && this.Tags.Contains(tag))
        {
            throw new ProductLifecycleException("Cannot remove tag from retired product.");
        }

        this.tags.Remove(tag);

        this.LastUpdated = DateTime.UtcNow;
    }

    public void Retire()
    {
        this.Retired = true;

        this.LastUpdated = DateTime.UtcNow;
    }

    public void Unretire()
    {
        this.Retired = false;
    }

    public void SetCategory(ProductCategory newCategory)
    {
        if (this.Retired && this.Category != newCategory)
        {
            throw new ProductLifecycleException("Can't add product to category. Product is retired");
        }

        this.Category = newCategory;

        this.LastUpdated = DateTime.UtcNow;
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
