using Answer.King.Domain;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Models;

[TestCategory(TestType.Unit)]
public class ProductTests
{
    private static readonly ProductFactory productFactory = new();

    [Fact]
    public void Product_InitWithDefaultId_ThrowsDefaultValueException()
    {
        // Arrange
        const int id = 0;
        const string productName = "Product Name";
        const string productDescription = "Product Description";
        var categories = new List<CategoryId> { new(1) };
        var tags = new List<TagId> { new(1) };
        const int price = 142;
        const bool retired = false;
        var createdOn = DateTime.Now;
        var lastUpdated = DateTime.Now;

        // Act / Assert

        Assert.Throws<Guard.DefaultValueException>(() => productFactory.CreateProduct(
            id,
            productName,
            productDescription,
            price,
            categories,
            tags,
            retired)
        );
    }

    [Fact]
    public void Product_InitWithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        const int id = 1;
        var productName = null as string;
        const string productDescription = "Product Description";
        var categories = new List<CategoryId> { new(1) };
        var tags = new List<TagId> { new(1) };
        const int price = 142;
        const bool retired = false;

        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => productFactory.CreateProduct(
            id,
            productName!,
            productDescription,
            price,
            categories,
            tags,
            retired)
        );
    }

    [Fact]
    public void Product_InitWithEmptyStringName_ThrowsEmptyStringException()
    {
        // Arrange
        const int id = 1;
        const string productName = "";
        const string productDescription = "Product Description";
        var categories = new List<CategoryId> { new(1) };
        var tags = new List<TagId> { new(1) };
        const int price = 142;
        const bool retired = false;

        // Act / Assert
        Assert.Throws<Guard.EmptyStringException>(() => productFactory.CreateProduct(
            id,
            productName,
            productDescription,
            price,
            categories,
            tags,
            retired)
        );
    }

    [Fact]
    public void Product_InitWithNullDescription_ThrowsArgumentNullException()
    {
        // Arrange
        const int id = 1;
        const string productName = "Product Name";
        var productDescription = null as string;
        var categories = new List<CategoryId> { new(1) };
        var tags = new List<TagId> { new(1) };
        const int price = 142;
        const bool retired = false;

        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => productFactory.CreateProduct(
            id,
            productName,
            productDescription!,
            price,
            categories,
            tags,
            retired)
        );
    }

    [Fact]
    public void Product_InitWithEmptyStringDescription_ThrowsEmptyStringException()
    {
        // Arrange
        const int id = 1;
        const string productName = "Product Name";
        const string productDescription = "";
        var categories = new List<CategoryId> { new(1) };
        var tags = new List<TagId> { new(1) };
        const int price = 142;
        const bool retired = false;

        // Act / Assert
        Assert.Throws<Guard.EmptyStringException>(() => productFactory.CreateProduct(
            id,
            productName,
            productDescription,
            price,
            categories,
            tags,
            retired)
        );
    }

    [Fact]
    public void Product_InitWithNegativePrice_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        const int id = 1;
        const string productName = "Product Name";
        const string productDescription = "Product Description";
        var categories = new List<CategoryId> { new(1) };
        var tags = new List<TagId> { new(1) };
        const int price = -1;
        const bool retired = false;

        // Act Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => productFactory.CreateProduct(
            id,
            productName,
            productDescription,
            price,
            categories,
            tags,
            retired)
        );
    }
}
