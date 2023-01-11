using System.Runtime.Serialization;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Orders.Models;

namespace Answer.King.Domain.Inventory;

public class Tag : IAggregateRoot
{
    public Tag(string name, string description, IList<ProductId> products)
    {
        Guard.AgainstNullOrEmptyArgument(nameof(name), name);
        Guard.AgainstNullOrEmptyArgument(nameof(description), description);
        Guard.AgainstNullArgument(nameof(products), products);

        this.Id = 0;
        this.Name = name;
        this.Description = description;
        this.LastUpdated = this.CreatedOn = DateTime.UtcNow;
        this._products = new HashSet<ProductId>(products);
        this.Retired = false;
    }

    // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
    private Tag(
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
        this._products = new HashSet<ProductId>(products);
        this.Retired = retired;
    }

    public long Id { get; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public DateTime CreatedOn { get; }

    public DateTime LastUpdated { get; private set; }

    private HashSet<ProductId> _products { get; }

    public IReadOnlyCollection<ProductId> Products => this._products;

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
            throw new TagLifecycleException("Cannot add product to retired tag.");
        }

        if (this._products.Add(productId))
        {
            this.LastUpdated = DateTime.UtcNow;
        }
    }

    public void RemoveProduct(ProductId productId)
    {
        if (this.Retired)
        {
            throw new TagLifecycleException("Cannot remove product from retired tag.");
        }

        if (this._products.Remove(productId))
        {
            this.LastUpdated = DateTime.UtcNow;
        }
    }

    public void RetireTag()
    {
        if (this.Retired)
        {
            throw new TagLifecycleException("The tag is already retired.");
        }

        if (this._products.Count > 0)
        {
            throw new TagLifecycleException(
                $"Cannot retire tag whilst there are still products assigned. {string.Join(',', this.Products.Select(p => p.Value))}");
        }

        this.Retired = true;

        this.LastUpdated = DateTime.UtcNow;
    }
}

[Serializable]
public class TagLifecycleException : Exception
{
    public TagLifecycleException(string message) : base(message)
    {
    }

    public TagLifecycleException() : base()
    {
    }

    public TagLifecycleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected TagLifecycleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
