using Answer.King.Api.Services;
using Answer.King.Domain.Repositories;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;
using Order = Answer.King.Domain.Orders.Order;
using Payment = Answer.King.Api.RequestModels.Payment;

namespace Answer.King.Api.UnitTests.Services;

[TestCategory(TestType.Unit)]
public class PaymentServiceTests
{
    private readonly IPaymentRepository paymentRepository = Substitute.For<IPaymentRepository>();

    private readonly IOrderRepository orderRepository = Substitute.For<IOrderRepository>();

    #region MakePayment

    [Fact]
    public async Task MakePayment_InvalidOrderIdReceived_ThrowsException()
    {
        // Arrange
        this.orderRepository.GetOne(Arg.Any<long>()).ReturnsNull();

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<PaymentServiceException>(() =>
            sut.MakePayment(new Payment()));
    }

    [Fact]
    public async Task MakePayment_PaymentAmountLessThanOrderTotal_ThrowsException()
    {
        // Arrange
        var order = new Order();
        order.AddLineItem(1, "product", "desc", 12.00, 2);

        var makePayment = new Payment { OrderId = order.Id, Amount = 20.00 };

        this.orderRepository.GetOne(Arg.Any<long>()).Returns(order);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<PaymentServiceException>(() =>
            sut.MakePayment(makePayment));
    }

    [Fact]
    public async Task MakePayment_PaidOrder_ThrowsException()
    {
        // Arrange
        var order = new Order();
        order.AddLineItem(1, "product", "desc", 12.00, 2);
        order.CompleteOrder();

        var makePayment = new Payment { OrderId = order.Id, Amount = 24.00 };

        this.orderRepository.GetOne(Arg.Any<long>()).Returns(order);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<PaymentServiceException>(() =>
            sut.MakePayment(makePayment));
    }

    [Fact]
    public async Task MakePayment_CancelledOrder_ThrowsException()
    {
        // Arrange
        var order = new Order();
        order.CancelOrder();

        var makePayment = new Payment { OrderId = order.Id, Amount = 24.00 };

        this.orderRepository.GetOne(Arg.Any<long>()).Returns(order);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<PaymentServiceException>(() =>
            sut.MakePayment(makePayment));
    }

    [Fact]
    public async Task MakePayment_ValidPaymentRequest_ReturnsPayment()
    {
        // Arrange
        var order = new Order();
        order.AddLineItem(1, "product", "desc", 12.00, 2);

        var makePayment = new Payment { OrderId = order.Id, Amount = 24.00 };
        var expectedPayment = new Domain.Repositories.Models.Payment(order.Id, makePayment.Amount, order.OrderTotal);

        this.orderRepository.GetOne(Arg.Any<long>()).Returns(order);

        // Act
        var sut = this.GetServiceUnderTest();
        var payment = await sut.MakePayment(makePayment);

        // Assert
        await this.orderRepository.Received().Save(order);
        await this.paymentRepository.Received().Add(payment);

        Assert.Equal(expectedPayment.Amount, payment.Amount);
        Assert.Equal(expectedPayment.Change, payment.Change);
        Assert.Equal(expectedPayment.OrderTotal, payment.OrderTotal);
        Assert.Equal(expectedPayment.OrderId, payment.OrderId);
    }

    #endregion

    #region Get

    [Fact]
    public async Task GetPayments_ReturnsAllPayments()
    {
        // Arrange
        var payments = new[]
        {
            new Domain.Repositories.Models.Payment(1, 50.00, 35.00),
            new Domain.Repositories.Models.Payment(1, 10.00, 7.95),
        };

        this.paymentRepository.GetAll().Returns(payments);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualPayments = await sut.GetPayments();

        // Assert
        Assert.Equal(payments, actualPayments);
        await this.paymentRepository.Received().GetAll();
    }

    [Fact]
    public async Task GetPayment_ValidPaymentId_ReturnsPayment()
    {
        // Arrange
        var payment = new Domain.Repositories.Models.Payment(1, 50.00, 35.00);

        this.paymentRepository.GetOne(payment.Id).Returns(payment);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualPayment = await sut.GetPayment(payment.Id);

        // Assert
        Assert.Equal(payment, actualPayment);
        await this.paymentRepository.Received().GetOne(payment.Id);
    }

    #endregion

    #region Setup

    private IPaymentService GetServiceUnderTest()
    {
        return new PaymentService(this.paymentRepository, this.orderRepository);
    }

    #endregion
}
