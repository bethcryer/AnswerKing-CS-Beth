using Answer.King.Api.RequestModels;
using Answer.King.Api.Services;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;
using Order = Answer.King.Domain.Orders.Order;
using OrderRequest = Answer.King.Api.RequestModels.Order;
using Product = Answer.King.Domain.Repositories.Models.Product;
using TagId = Answer.King.Domain.Repositories.Models.TagId;

namespace Answer.King.Api.UnitTests.Services;

[TestCategory(TestType.Unit)]
public class OrderServiceTests
{
    private readonly IOrderRepository orderRepository = Substitute.For<IOrderRepository>();

    private readonly IProductRepository productRepository = Substitute.For<IProductRepository>();

    private readonly ICategoryRepository categoryRepository = Substitute.For<ICategoryRepository>();

    private static CategoryFactory CategoryFactory { get; } = new();

    private static ProductFactory ProductFactory { get; } = new();

    #region Create

    [Fact]
    public async Task CreateOrder_InvalidProductsSubmitted_ThrowsException()
    {
        // Arrange
        var lineItem1 = new LineItem
        {
            ProductId = 1,
            Quantity = 1,
        };

        var lineItem2 = new LineItem
        {
            ProductId = 1,
            Quantity = 1,
        };

        var orderRequest = new OrderRequest
        {
            LineItems = new List<LineItem>(new[] { lineItem1, lineItem2 }),
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductInvalidException>(
            () => sut.CreateOrder(orderRequest));
    }

    [Fact]
    public async Task CreateOrder_ValidOrderRequestRecieved_ReturnsOrder()
    {
        // Arrange
        var category = new ProductCategory(1, "name", "description");
        var tagIds = new List<TagId> { new(1) };
        var products = new[]
        {
            ProductFactory.CreateProduct(1, "product 1", "desc", 2.0, category, tagIds, false),
            ProductFactory.CreateProduct(2, "product 2", "desc", 4.0, category, tagIds, false),
        };

        var orderRequest = new OrderRequest
        {
            LineItems = new List<LineItem>(new[]
            {
                new LineItem { ProductId = products[0].Id, Quantity = 4 },
                new LineItem { ProductId = products[1].Id, Quantity = 1 },
            }),
        };

        var now = DateTime.UtcNow;
        var categories = new[]
        {
            CategoryFactory.CreateCategory(
                1,
                "category 1",
                "desc",
                now,
                now,
                new[] { new ProductId(1) },
                false),
        };

        this.productRepository.GetMany(Arg.Any<IList<long>>()).Returns(products);
        this.categoryRepository.GetByProductId(Arg.Any<long[]>()).Returns(categories);

        // Act
        var sut = this.GetServiceUnderTest();
        var createdOrder = await sut.CreateOrder(orderRequest);

        // Assert
        Assert.Equal(2, createdOrder.LineItems.Count);
        Assert.Equal(12.0, createdOrder.OrderTotal);
    }

    #endregion

    #region Update

    [Fact]
    public async Task UpdateOrder_InvalidOrderIdReceived_ReturnsNull()
    {
        // Arrange
        this.orderRepository.GetOne(Arg.Any<long>()).ReturnsNull();

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        Assert.Null(await sut.UpdateOrder(1, new OrderRequest()));
    }

    [Fact]
    public async Task UpdateOrder_ValidOrderRequestReceived_ReturnsUpdatedOrder()
    {
        // Arrange
        var order = new Order();
        this.orderRepository.GetOne(Arg.Any<long>()).Returns(order);

        var category = new ProductCategory(1, "name", "description");
        var tagIds = new List<TagId> { new(1) };
        var products = new[]
        {
            ProductFactory.CreateProduct(1, "product 1", "desc", 2.0, category, tagIds, false),
            ProductFactory.CreateProduct(2, "product 2", "desc", 4.0, category, tagIds, false),
        };

        var orderRequest = new OrderRequest
        {
            LineItems = new List<LineItem>(new[]
            {
                new LineItem { ProductId = products[0].Id, Quantity = 4 },
            }),
        };

        var now = DateTime.UtcNow;
        var categories = new[]
        {
            CategoryFactory.CreateCategory(
                1,
                "category 1",
                "desc",
                now,
                now,
                new[] { new ProductId(1) },
                false),
        };

        this.productRepository.GetMany(Arg.Any<IList<long>>()).Returns(products);
        this.categoryRepository.GetByProductId(Arg.Any<long[]>()).Returns(categories);

        // Act
        var sut = this.GetServiceUnderTest();
        var updatedOrder = await sut.UpdateOrder(1, orderRequest);

        // Assert
        await this.orderRepository.Received().Save(Arg.Any<Order>());

        Assert.Equal(1, updatedOrder!.LineItems.Count);
        Assert.Equal(8.0, updatedOrder.OrderTotal);
    }

    [Fact]
    public async Task UpdateOrder_InvalidProductReceivedInOrder_ThrowsException()
    {
        // Arrange
        var order = new Order();
        this.orderRepository.GetOne(Arg.Any<long>()).Returns(order);

        var products = new[]
        {
            new Product("product 1", "desc", 2.0, new ProductCategory(1, "name", "description")),
            new Product("product 2", "desc", 4.0, new ProductCategory(1, "name", "description")),
        };

        var orderRequest = new OrderRequest
        {
            LineItems = new List<LineItem>(new[]
            {
                new LineItem { ProductId = 1, Quantity = 4 },
            }),
        };

        this.productRepository.GetMany(Arg.Any<IList<long>>()).Returns(products);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductInvalidException>(() =>
            sut.UpdateOrder(1, orderRequest));
    }

    #endregion

    #region Get

    [Fact]
    public async Task GetOrder_ValidOrderId_ReturnsOrder()
    {
        // Arrange
        var order = new Order();
        this.orderRepository.GetOne(order.Id).Returns(order);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualOrder = await sut.GetOrder(order.Id);

        // Assert
        Assert.Equal(order, actualOrder);
        await this.orderRepository.Received().GetOne(order.Id);
    }

    [Fact]
    public async Task GetOrders_ReturnsAllOrders()
    {
        // Arrange
        var orders = new[]
        {
            new Order(), new Order(),
        };

        this.orderRepository.GetAll().Returns(orders);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualOrders = await sut.GetOrders();

        // Assert
        Assert.Equal(orders, actualOrders);
        await this.orderRepository.Received().GetAll();
    }

    #endregion

    #region Cancel

    [Fact]
    public async Task CancelOrder_InvalidOrderIdReceived_ReturnsNull()
    {
        // Arrange
        this.orderRepository.GetOne(Arg.Any<long>()).ReturnsNull();

        // Act
        var sut = this.GetServiceUnderTest();
        var cancelOrder = await sut.CancelOrder(1);

        // Assert
        Assert.Null(cancelOrder);
    }

    #endregion

    #region Setup

    private IOrderService GetServiceUnderTest()
    {
        return new OrderService(this.orderRepository, this.productRepository);
    }

    #endregion
}
