using System.Runtime.Serialization;
using Answer.King.Domain.Inventory.Models;

namespace Answer.King.Domain.Inventory;

public class Category : IAggregateRoot
{
    private readonly HashSet<ProductId> products;

    public Category(string name, string description, IList<ProductId> products)
    {
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNullArgument(nameof(products), products);

        this.Id = 0;
        this.Name = name;
        this.Description = description;
        this.LastUpdated = this.CreatedOn = DateTime.UtcNow;
        this.products = new HashSet<ProductId>(products);
        this.Retired = false;
    }

    // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
    private Category(
        long id,
        string name,
        string description,
        DateTime createdOn,
        DateTime lastUpdated,
        IList<ProductId> products,
        bool retired)
    {
#pragma warning restore IDE0051 // Remove unused private members
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstDefaultValue(nameof(createdOn), createdOn);
        Guard.AgainstDefaultValue(nameof(lastUpdated), lastUpdated);
        Guard.AgainstNullArgument(nameof(products), products);

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.CreatedOn = createdOn;
        this.LastUpdated = lastUpdated;
        this.products = new HashSet<ProductId>(products);
        this.Retired = retired;
    }

    public long Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public DateTime CreatedOn { get; }

    public DateTime LastUpdated { get; private set; }

    public IReadOnlyCollection<ProductId> Products => this.products;

    public bool Retired { get; private set; }

    public void Rename(string name, string description)
    {
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);

        this.Name = name;
        this.Description = description;
        this.LastUpdated = DateTime.UtcNow;
    }

    public void AddProduct(ProductId productId)
    {
        if (this.Retired)
        {
            throw new CategoryLifecycleException("Cannot add product to retired category.");
        }

        if (this.products.Add(productId))
        {
            this.LastUpdated = DateTime.UtcNow;
        }
    }

    public void RemoveProduct(ProductId productId)
    {
        if (this.Retired)
        {
            throw new CategoryLifecycleException("Cannot remove product from retired category.");
        }

        if (this.products.Remove(productId))
        {
            this.LastUpdated = DateTime.UtcNow;
        }
    }

    public void RetireCategory()
    {
        if (this.Retired)
        {
            throw new CategoryLifecycleException("The category is already retired.");
        }

        if (this.products.Count > 0)
        {
            throw new CategoryLifecycleException(
                $"Cannot retire category whilst there are still products assigned. {string.Join(',', this.Products.Select(p => p.Value))}");
        }

        this.Retired = true;

        this.LastUpdated = DateTime.UtcNow;
    }

    public void UnretireCategory()
    {
        if (!this.Retired)
        {
            throw new CategoryLifecycleException("The category is not retired.");
        }

        this.Retired = false;

        this.LastUpdated = DateTime.UtcNow;
    }
}

[Serializable]
public class CategoryLifecycleException : Exception
{
    public CategoryLifecycleException(string message)
        : base(message)
    {
    }

    public CategoryLifecycleException()
    {
    }

    public CategoryLifecycleException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected CategoryLifecycleException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
