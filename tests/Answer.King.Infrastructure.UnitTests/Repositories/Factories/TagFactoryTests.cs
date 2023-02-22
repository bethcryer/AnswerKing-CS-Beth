using System.Reflection;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Infrastructure.UnitTests.TestObjects;
using Answer.King.Test.Common.CustomTraits;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Factories;

[UsesVerify]
[TestCategory(TestType.Unit)]
public class TagFactoryTests
{
    private static readonly TagFactory TagFactory = new();

    [Fact]
    public Task CreateTag_ConstructorExists_ReturnsTag()
    {
        // Arrange / Act
        var now = DateTime.UtcNow;
        var result = TagFactory.CreateTag(1, "NAME", "DESC", now, now, new List<ProductId>(), false);

        // Assert
        Assert.IsType<Tag>(result);
        return Verify(result);
    }

    [Fact]
    public void CreateTag_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var tagFactoryConstructorPropertyInfo =
        typeof(TagFactory).GetField("<TagConstructor>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

        var constructor = tagFactoryConstructorPropertyInfo?.GetValue(TagFactory);

        var wrongConstructor = typeof(WrongConstructor).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        tagFactoryConstructorPropertyInfo?.SetValue(TagFactory, wrongConstructor);

        var now = DateTime.UtcNow;

        // Act // Assert
        Assert.Throws<ArgumentException>(() =>
            TagFactory.CreateTag(1, "NAME", "DESC", now, now, new List<ProductId>(), false));

        tagFactoryConstructorPropertyInfo?.SetValue(TagFactory, constructor);
    }
}
