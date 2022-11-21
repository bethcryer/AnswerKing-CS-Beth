using Answer.King.Domain;
using Answer.King.Domain.Inventory.Models;

namespace Answer.King.Api.IntegrationTests.Common.Models;

public class Category
{
    public Category(
        long id,
        string name,
        string description,
        DateTime createdOn,
        DateTime lastUpdated,
        IList<ProductId>? products,
        bool retired)
    {

        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.CreatedOn = createdOn;
        this.LastUpdated = lastUpdated;
        this.Products = products ?? new List<ProductId>();
        this.Retired = retired;
    }


    public long Id { get; }

    public string Name { get; }

    public string Description { get; }

    public DateTime CreatedOn { get; }

    public DateTime LastUpdated { get; }

    public IList<ProductId> Products { get; }

    public bool Retired { get; }

}
