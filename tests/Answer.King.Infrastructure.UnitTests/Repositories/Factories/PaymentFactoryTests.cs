using System.Reflection;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Infrastructure.UnitTests.TestObjects;
using Answer.King.Test.Common.CustomTraits;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Factories;

[UsesVerify]
[TestCategory(TestType.Unit)]
public class PaymentFactoryTests
{
    private static readonly PaymentFactory PaymentFactory = new();

    [Fact]
    public Task CreatePayment_ConstructorExists_ReturnsPayment()
    {
        // Arrange / Act
        var result = PaymentFactory.CreatePayment(1, 1, 1, 1, DateTime.UtcNow);

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

        var constructor = paymentFactoryConstructorPropertyInfo?.GetValue(PaymentFactory);

        var wrongConstructor = typeof(WrongConstructor).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .SingleOrDefault(c => c.IsPrivate && c.GetParameters().Length > 0);

        paymentFactoryConstructorPropertyInfo?.SetValue(PaymentFactory, wrongConstructor);

        // Act // Assert
        Assert.Throws<TargetParameterCountException>(() =>
            PaymentFactory.CreatePayment(1, 1, 1, 1, DateTime.UtcNow));

        paymentFactoryConstructorPropertyInfo?.SetValue(PaymentFactory, constructor);
    }
}
