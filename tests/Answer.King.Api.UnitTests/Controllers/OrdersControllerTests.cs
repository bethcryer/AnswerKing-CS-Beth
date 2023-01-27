using Answer.King.Api.Controllers;
using Answer.King.Api.Services;
using Answer.King.Test.Common.CustomAsserts;
using Answer.King.Test.Common.CustomTraits;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Xunit;
using InputOrder = Answer.King.Api.RequestModels.Order;
using LineItem = Answer.King.Api.RequestModels.LineItem;
using OutputOrder = Answer.King.Domain.Orders.Order;

namespace Answer.King.Api.UnitTests.Controllers;

[TestCategory(TestType.Unit)]
public class OrdersControllerTests
{
    #region Setup

    private static readonly IOrderService OrderService = Substitute.For<IOrderService>();

    private static readonly OrdersController GetSubjectUnderTest =
        new(OrderService);

    #endregion Setup

    #region GenericControllerTests

    [Fact]
    public void Controller_RouteAttribute_IsPresentWithCorrectRoute()
    {
        // Assert
        AssertController.HasRouteAttribute<OrdersController>("api/[controller]");
        Assert.Equal("OrdersController", nameof(OrdersController));
    }

    #endregion GenericControllerTests

    #region GetAll

    [Fact]
    public void GetAll_Method_HasCorrectVerbAttribute()
    {
        // Assert
        AssertController.MethodHasVerb<OrdersController, HttpGetAttribute>(
            nameof(OrdersController.GetAll));
    }

    [Fact]
    public async Task GetAll_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        var data = new List<OutputOrder>();
        OrderService.GetOrders().Returns(data);

        // Act
        var result = await GetSubjectUnderTest.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion GetAll

    #region GetOne

    [Fact]
    public void GetOne_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<OrdersController, HttpGetAttribute>(
            nameof(OrdersController.GetOne), "{id}");
    }

    [Fact]
    public async Task GetOne_ValidRequestWithNullResult_ReturnsNotFoundResult()
    {
        // Arrange
        OutputOrder data = null!;
        OrderService.GetOrder(Arg.Any<long>()).Returns(data);

        // Act
        var result = await GetSubjectUnderTest.GetOne(Arg.Any<long>());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetOne_ValidRequestWithResult_ReturnsOkObjectResult()
    {
        // Arrange
        const long id = 1;
        var data = new OutputOrder();
        OrderService.GetOrder(Arg.Is(id)).Returns(data);

        // Act
        var result = await GetSubjectUnderTest.GetOne(id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion GetOne

    #region Post

    [Fact]
    public void Post_Method_HasCorrectVerbAttribute()
    {
        // Assert
        AssertController.MethodHasVerb<OrdersController, HttpPostAttribute>(
            nameof(OrdersController.Post));
    }

    [Fact]
    public async Task Post_ValidRequestCallsGetAction_ReturnsNewOrder()
    {
        // Arrange
        var orderRequestModel = new InputOrder { LineItems = new List<LineItem> { new LineItem { ProductId = 1, Quantity = 1 } } };

        var order = new OutputOrder();
        order.AddLineItem(1, "PRODUCT_NAME", "PRODUCT_DESC", 0);

        OrderService.CreateOrder(orderRequestModel).Returns(order);

        // Act
        var result = await GetSubjectUnderTest.Post(orderRequestModel);

        // Assert
        await OrderService.Received().CreateOrder(orderRequestModel);
        Assert.IsType<CreatedAtActionResult>(result);
    }

    #endregion Post

    #region Put

    [Fact]
    public void Put_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<OrdersController, HttpPutAttribute>(
            nameof(OrdersController.Put), "{id}");
    }

    [Fact]
    public async Task Put_NullOrder_ReturnsNotFoundResult()
    {
        // Arrange
        const int id = 1;

        // Act
        var result = await GetSubjectUnderTest.Put(id, null!);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Put_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        const int id = 1;
        var orderRequestModel = new InputOrder { LineItems = new List<LineItem> { new LineItem { ProductId = 1, Quantity = 1 } } };

        var order = new OutputOrder();
        order.AddLineItem(1, "PRODUCT_NAME", "PRODUCT_NAME", 0);

        OrderService.UpdateOrder(id, orderRequestModel).Returns(order);

        // Act
        var result = await GetSubjectUnderTest.Put(id, orderRequestModel);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion Put

    #region Cancel

    [Fact]
    public void Cancel_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<OrdersController, HttpDeleteAttribute>(
            nameof(OrdersController.Cancel), "{id}");
    }

    [Fact]
    public async Task Cancel_NullOrder_ReturnsNotFound()
    {
        // Arrange / Act
        var result = await GetSubjectUnderTest.Cancel(Arg.Any<long>());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Cancel_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        const int id = 1;
        var order = new OutputOrder();
        order.CancelOrder();

        OrderService.CancelOrder(id).Returns(order);

        // Act
        var result = await GetSubjectUnderTest.Cancel(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    #endregion Cancel

}
