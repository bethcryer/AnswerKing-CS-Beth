namespace Answer.King.Domain.Repositories.Models;

public record Category
{
    public Category(long id, string name, string description)
    {
        Guard.AgainstDefaultValue(nameof(id), id);
        Guard.AgainstNullOrWhitespaceArgument(nameof(name), name);
        Guard.AgainstNullOrWhitespaceArgument(nameof(description), description);

        this.Id = id;
        this.Name = name;
        this.Description = description;
    }

    public long Id { get; }

    public string Name { get; }

    public string Description { get; }
}
