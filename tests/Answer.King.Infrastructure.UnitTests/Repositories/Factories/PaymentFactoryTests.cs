using System.Reflection;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Factories;

[UsesVerify]
[TestCategory(TestType.Unit)]
public class PaymentFactoryTests
{
    private static readonly PaymentFactory paymentFactory = new();

    [Fact]
    public Task CreatePayment_ConstructorExists_ReturnsPayment()
    {
        // Arrange / Act
        var result = paymentFactory.CreatePayment(1, 1, 1, 1, DateTime.UtcNow);

        // Assert
        Assert.IsType<Payment>(result);
        return Verify(result);
    }

    [Fact]
    public void CreatePayment_ConstructorNotFound_ReturnsException()
    {
        // Arrange
        var paymentFactoryConstructorPropertyInfo =
        typeof(PaymentFactory).GetField("<PaymentConstructor>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

        var constructor = paymentFactoryConstructorPropertyInfo?.GetValue(paymentFactory);

        var wrongConstructor = typeof(Category).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        paymentFactoryConstructorPropertyInfo?.SetValue(paymentFactory, wrongConstructor);

        // Act // Assert
        Assert.Throws<TargetParameterCountException>(() =>
            paymentFactory.CreatePayment(1, 1, 1, 1, DateTime.UtcNow));

        paymentFactoryConstructorPropertyInfo?.SetValue(paymentFactory, constructor);
    }
}
