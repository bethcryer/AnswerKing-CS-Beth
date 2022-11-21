using Answer.King.Domain.Orders.Models;
using Answer.King.Test.Common.CustomTraits;
using Xunit;
using Product = Answer.King.Domain.Orders.Models.Product;

namespace Answer.King.Domain.UnitTests.Orders.Models;

[TestCategory(TestType.Unit)]
public class LineItemTests
{
    [Fact]
    public void LineItem_InitWithNullProduct_ThrowsArgumentNullException()
    {
        // Arrange
        var product = null as Product;

        // Act / Assert
        Assert.Throws<ArgumentNullException>(() => new LineItem(product!));
    }

    [Fact]
    public void AddQuantity_ValidQuantity_IncrementsByCorrectAmount()
    {
        // Arrange
        var product = this.GetProduct();
        var lineItem = new LineItem(product);
        var quantity = 5;
        var expected = quantity;

        // Act
        lineItem.AddQuantity(quantity);

        // Assert
        Assert.Equal(expected, lineItem.Quantity);
    }

    [Fact]
    public void AddQuantity_AddZero_ThrowsLineItemException()
    {
        // Arrange
        var product = this.GetProduct();
        var lineItem = new LineItem(product);

        // Act / Assert
        Assert.Throws<LineItemException>(() => lineItem.AddQuantity(0));
    }

    [Fact]
    public void AddQuantity_InvalidQuantity_ThrowsLineItemException()
    {
        // Arrange
        var product = this.GetProduct();
        var lineItem = new LineItem(product);
        var quantity = -3;

        // Act / Assert
        Assert.Throws<LineItemException>(() => lineItem.AddQuantity(quantity));
    }

    [Fact]
    public void RemoveQuantity_ValidQuantity_DecrementsByCorrectAmount()
    {
        // Arrange
        var product = this.GetProduct();
        var lineItem = new LineItem(product);
        var quantityToAdd = 5;
        var quantityToRemove = 3;
        var expected = quantityToAdd - quantityToRemove;

        // Act
        lineItem.AddQuantity(quantityToAdd);
        lineItem.RemoveQuantity(quantityToRemove);

        // Assert
        Assert.Equal(expected, lineItem.Quantity);
    }

    [Fact]
    public void RemoveQuantity_InvalidQuantity_ThrowsLineItemException()
    {
        // Arrange
        var product = this.GetProduct();
        var lineItem = new LineItem(product);
        var quantity = -3;

        // Act / Assert
        Assert.Throws<LineItemException>(() => lineItem.RemoveQuantity(quantity));
    }

    [Fact]
    public void RemoveQuantity_RemoveZero_ThrowsLineItemException()
    {
        // Arrange
        var product = this.GetProduct();
        var lineItem = new LineItem(product);

        // Act / Assert
        Assert.Throws<LineItemException>(() => lineItem.RemoveQuantity(0));
    }

    [Fact]
    public void RemoveQuantity_RemoveMoreThanQuantity_ThrowsLineItemException()
    {
        // Arrange
        var product = this.GetProduct();
        var lineItem = new LineItem(product);
        var quantity = 1;
        var moreThanQuantity = 3;

        // Act
        lineItem.AddQuantity(quantity);

        // Act / Assert
        Assert.Throws<LineItemException>(() => lineItem.RemoveQuantity(moreThanQuantity));
    }

    [Fact]
    public void SubTotal_WithPriceAndQuantity_CalculatesCorrectPrice()
    {
        // Arrange
        var product = this.GetProduct();
        var lineItem = new LineItem(product);
        var quantity = 5;
        var expected = product.Price * quantity;

        // Act
        lineItem.AddQuantity(quantity);

        // Assert
        Assert.Equal(expected, lineItem.SubTotal);
    }

    #region Helpers

    private Product GetProduct() => new Product(
        1,
        "name",
        "description",
        142,
        new List<Category>
        {
            new Category(1, "name", "description")
        }
    );

    #endregion Helpers
}
