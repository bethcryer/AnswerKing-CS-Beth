using System.Collections;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Domain.UnitTests.Orders;

[TestCategory(TestType.Unit)]
public class OrderTests
{
    [Fact]
    public void OrderStateStateEnum_MapsToCorrectInt()
    {
        var totalStreamNamesTested = new OrderStateConstantsData().Count();
        var totalConstants = GetAll().Count();

        Assert.Equal(totalStreamNamesTested, totalConstants);
    }

    [Fact]
    public void CompleteOrder_OrderStatusCancelled_ThrowsOrderLifecycleException()
    {
        var order = new Order();
        order.CancelOrder();

        Assert.Throws<OrderLifeCycleException>(() => order.CompleteOrder());
    }

    [Fact]
    public void CancelOrder_OrderStatusCompleted_ThrowsOrderLifecycleException()
    {
        var order = new Order();
        order.CompleteOrder();

        Assert.Throws<OrderLifeCycleException>(() => order.CancelOrder());
    }

    #region AddLineItem

    [Fact]
    public void AddLineItem_OrderStatusCompleted_ThrowsOrderLifecycleException()
    {
        // Arrange
        var order = new Order();
        const int id = 1;
        const string name = "name";
        const string description = "description";
        const double price = 1.24;
        const int quantity = 2;

        order.CompleteOrder();

        // Act / Assert
        Assert.Throws<OrderLifeCycleException>(() =>
            order.AddLineItem(id, name, description, price, quantity));
    }

    [Fact]
    public void AddLineItem_OrderStatusCancelled_ThrowsOrderLifecycleException()
    {
        // Arrange
        var order = new Order();
        const int id = 1;
        const string name = "name";
        const string description = "description";
        const double price = 1.24;
        const int quantity = 2;

        order.CancelOrder();

        // Act / Assert
        Assert.Throws<OrderLifeCycleException>(() =>
            order.AddLineItem(id, name, description, price, quantity));
    }

    [Fact]
    public void AddLineItem_ValidArgumentsWithNewItem_AddsToLineItemsWithCorrectQuantity()
    {
        // Arrange
        var order = new Order();
        const int id = 1;
        const string name = "name";
        const string description = "description";
        const double price = 1.24;
        const int quantity = 2;

        // Act
        order.AddLineItem(id, name, description, price, quantity);

        var lineItem = order.LineItems.FirstOrDefault();

        // Assert
        Assert.NotNull(lineItem);
        Assert.Equal(quantity, lineItem.Quantity);
        Assert.NotNull(lineItem.Product);
        Assert.Equal(id, lineItem.Product.Id);
        Assert.Equal(price, lineItem.Product.Price);
    }

    [Fact]
    public void AddLineItem_ValidArgumentsWithNewItem_AddsToLineItemsAndCalculatesTheCorrectSubtotal()
    {
        // Arrange
        var order = new Order();
        const int id = 1;
        const string name = "name";
        const string description = "description";
        const double price = 1.24;
        const int quantity = 2;

        // Act
        order.AddLineItem(id, name, description, price, quantity);

        var lineItem = order.LineItems.FirstOrDefault();

        // Assert
        Assert.NotNull(lineItem);
        Assert.Equal(quantity * price, lineItem.SubTotal);
    }

    [Fact]
    public void AddLineItem_ValidArgumentsWithNewItem_AddsToLineItemsWithCorrectPrice()
    {
        // Arrange
        var order = new Order();
        const int id = 1;
        const string name = "name";
        const string description = "description";
        const double price = 1.24;
        const int quantity = 2;

        // Act
        order.AddLineItem(id, name, description, price, quantity);

        var lineItem = order.LineItems.FirstOrDefault();

        // Assert
        Assert.NotNull(lineItem);
        Assert.NotNull(lineItem.Product);
        Assert.Equal(price, lineItem.Product.Price);
    }

    #endregion AddLineItem

    #region RemoveLineItem

    [Fact]
    public void RemoveLineItem_OrderStatusCompleted_ThrowsOrderLifecycleException()
    {
        // Arrange
        var order = new Order();
        const int id = 1;
        const int quantity = 2;

        order.CompleteOrder();

        // Act / Assert
        Assert.Throws<OrderLifeCycleException>(() =>
            order.RemoveLineItem(id, quantity));
    }

    [Fact]
    public void RemoveLineItem_OrderStatusCancel_ThrowsOrderLifecycleException()
    {
        // Arrange
        var order = new Order();
        const int id = 1;
        const int quantity = 2;

        order.CancelOrder();

        // Act / Assert
        Assert.Throws<OrderLifeCycleException>(() =>
            order.RemoveLineItem(id, quantity));
    }

    [Fact]
    public void RemoveLineItem_LineItemDoesNotExistInOrder_DoesNotAttemptToRemoveFromOrder()
    {
        // Arrange
        var order = new Order();
        const int id = 1;
        const int quantity = 3;

        // Act
        order.RemoveLineItem(id, quantity);

        // Assert
        Assert.Empty(order.LineItems);
    }

    [Fact]
    public void RemoveLineItem_LineItemExistsInOrder_DecrementCorrectQuantityValue()
    {
        // Arrange
        var order = new Order();
        const int id = 1;
        const string productName = "PRODUCT_NAME";
        const string productDescription = "PRODUCT_DESCRIPTION";
        const int quantity = 5;
        const double price = 1.25;

        order.AddLineItem(id, productName, productDescription, price, quantity);

        // Act
        order.RemoveLineItem(id, 3);

        var lineItem = order.LineItems.FirstOrDefault();

        // Assert
        Assert.NotNull(lineItem);
        Assert.Equal(2, lineItem.Quantity);
    }

    [Fact]
    public void RemoveLineItem_LineItemExistsInOrder_RemovedFromOrderIfQuantityGetCurrent()
    {
        // Arrange
        var order = new Order();
        const int id = 1;
        const string productName = "PRODUCT_NAME";
        const string productDescription = "PRODUCT_DESCRIPTION";
        const int quantity = 3;
        const double price = 1.25;

        order.AddLineItem(id, productName, productDescription, price, quantity);

        // Act
        order.RemoveLineItem(id, 3);

        var lineItem = order.LineItems.FirstOrDefault();

        // Assert
        Assert.Null(lineItem);
        Assert.Equal(0, order.LineItems.Count);
    }

    #endregion RemoveLineItem

    #region Helpers

    private static IEnumerable<string> GetAll()
    {
        var enumValues = Enum.GetNames(typeof(OrderStatus));

        return enumValues;
    }

    #endregion Helpers
}

#region ClassData

public class OrderStateConstantsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        return new List<object[]>
        {
            new object[] { 0, OrderStatus.Created },
            new object[] { 1, OrderStatus.Complete },
            new object[] { 2, OrderStatus.Cancelled },
        }.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

#endregion ClassData
