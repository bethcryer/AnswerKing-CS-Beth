using System.Reflection;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Factories;

[UsesVerify]
[TestCategory(TestType.Unit)]
public class ProductFactoryTests
{
    private static readonly ProductFactory productFactory = new();

    [Fact]
    public Task CreateProduct_ConstructorExists_ReturnsProduct()
    {
        // Arrange / Act
        var result = productFactory.CreateProduct(1, "NAME", "DESC", 1, new List<CategoryId>(), new List<TagId>(), false);

        // Assert
        Assert.IsType<Product>(result);
        return Verify(result);
    }

    [Fact]
    public void CreateProduct_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var productFactoryConstructorPropertyInfo =
        typeof(ProductFactory).GetField("<ProductConstructor>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

        var constructor = productFactoryConstructorPropertyInfo?.GetValue(productFactory);

        var wrongConstructor = typeof(Category).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        productFactoryConstructorPropertyInfo?.SetValue(productFactory, wrongConstructor);

        // Act // Assert
        Assert.Throws<ArgumentException>(() =>
            productFactory.CreateProduct(1, "NAME", "DESC", 1, new List<CategoryId>(), new List<TagId>(), false));

        productFactoryConstructorPropertyInfo?.SetValue(productFactory, constructor);
    }
}
