using Answer.King.Domain.Repositories.Models;
using Answer.King.Test.Common.CustomTraits;
using Answer.King.Infrastructure.Repositories.Mappings;
using Xunit;

namespace Answer.King.Domain.UnitTests.Repositories.Models;

[TestCategory(TestType.Unit)]
public class ProductTests
{
    [Fact]
    public void Product_InitWithDefaultId_ThrowsDefaultValueException()
    {
        // Arrange
        var id = 0;
        var productName = "Product Name";
        var productDescription = "Product Description";
        var categories = new List<Category> { this.GetCategory() };
        var price = 142;
        var retired = false;
        var createdOn = DateTime.Now;
        var lastUpdated = DateTime.Now;

        // Act / Assert

        Assert.Throws<Guard.DefaultValueException>(() => ProductFactory.CreateProduct(
            id,
            productName,
            productDescription,
            price,
            categories,
            retired)
        );
    }

    [Fact]
    public void Product_InitWithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var id = 1;
        var productName = null as string;
        var productDescription = "Product Description";
        var categories = new List<Category> { this.GetCategory() };
        var price = 142;
        var retired = false;

        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => ProductFactory.CreateProduct(
            id,
            productName!,
            productDescription,
            price,
            categories,
            retired)
        );
    }

    [Fact]
    public void Product_InitWithEmptyStringName_ThrowsEmptyStringException()
    {
        // Arrange
        var id = 1;
        var productName = "";
        var productDescription = "Product Description";
        var categories = new List<Category> { this.GetCategory() };
        var price = 142;
        var retired = false;

        // Act / Assert
        Assert.Throws<Guard.EmptyStringException>(() => ProductFactory.CreateProduct(
            id,
            productName,
            productDescription,
            price,
            categories,
            retired)
        );
    }

    [Fact]
    public void Product_InitWithNullDescription_ThrowsArgumentNullException()
    {
        // Arrange
        var id = 1;
        var productName = "Product Name";
        var productDescription = null as string;
        var categories = new List<Category> { this.GetCategory() };
        var price = 142;
        var retired = false;

        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => ProductFactory.CreateProduct(
            id,
            productName,
            productDescription!,
            price,
            categories,
            retired)
        );
    }

    [Fact]
    public void Product_InitWithEmptyStringDescription_ThrowsEmptyStringException()
    {
        // Arrange
        var id = 1;
        var productName = "Product Name";
        var productDescription = "";
        var categories = new List<Category> { this.GetCategory() };
        var price = 142;
        var retired = false;

        // Act / Assert
        Assert.Throws<Guard.EmptyStringException>(() => ProductFactory.CreateProduct(
            id,
            productName,
            productDescription,
            price,
            categories,
            retired)
        );
    }

    [Fact]
    public void Product_InitWithNegativePrice_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var id = 1;
        var productName = "Product Name";
        var productDescription = "Product Description";
        var categories = new List<Category> { this.GetCategory() };
        var price = -1;
        var retired = false;

        // Act Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => ProductFactory.CreateProduct(
            id,
            productName,
            productDescription,
            price,
            categories,
            retired)
        );
    }

    #region Helpers

    private Category GetCategory() => new Category(
        1,
        "name",
        "description"
    );

    #endregion Helpers
}
