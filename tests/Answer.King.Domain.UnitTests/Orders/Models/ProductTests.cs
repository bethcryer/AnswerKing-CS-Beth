using Answer.King.Test.Common.CustomTraits;
using Xunit;
using Product = Answer.King.Domain.Orders.Models.Product;

namespace Answer.King.Domain.UnitTests.Orders.Models;

[TestCategory(TestType.Unit)]
public class ProductTests
{
    [Fact]
    public void Product_InitWithDefaultId_ThrowsDefaultValueException()
    {
        // Arrange
        const int id = 0;
        const string name = "name";
        const string description = "description";
        const int price = 142;

        // Act / Assert
        Assert.Throws<Guard.DefaultValueException>(() => new Product(
            id,
            name,
            description,
            price));
    }

    [Fact]
    public void Product_InitWithNegativePrice_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const int id = 1;
        const string name = "name";
        const string description = "description";
        const int price = -1;

        // Act Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Product(
            id,
            name,
            description,
            price));
    }
}
