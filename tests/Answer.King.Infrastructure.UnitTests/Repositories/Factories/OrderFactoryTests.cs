using System.Reflection;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Factories;

[UsesVerify]
[TestCategory(TestType.Unit)]
public class OrderFactoryTests
{
    private static readonly OrderFactory OrderFactory = new();

    [Fact]
    public Task CreateOrder_ConstructorExists_ReturnsOrder()
    {
        // Arrange / Act
        var now = DateTime.UtcNow;
        var result = OrderFactory.CreateOrder(1, now, now, OrderStatus.Created, new List<LineItem>());

        // Assert
        Assert.IsType<Order>(result);
        return Verify(result);
    }

    [Fact]
    public void CreateOrder_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var orderFactoryConstructorPropertyInfo =
        typeof(OrderFactory).GetField("<OrderConstructor>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

        var constructor = orderFactoryConstructorPropertyInfo?.GetValue(OrderFactory);

        var wrongConstructor = typeof(Domain.Inventory.Category).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        orderFactoryConstructorPropertyInfo?.SetValue(OrderFactory, wrongConstructor);

        var now = DateTime.UtcNow;

        // Act // Assert
        Assert.Throws<TargetParameterCountException>(() =>
            OrderFactory.CreateOrder(1, now, now, OrderStatus.Created, new List<LineItem>()));

        // Reset static constructor to correct value
        orderFactoryConstructorPropertyInfo?.SetValue(OrderFactory, constructor);
    }
}
