using Answer.King.Api.IntegrationTests.Common;
using Order = Answer.King.Api.IntegrationTests.Common.Models.Order;
using RMLineItems = Answer.King.Api.RequestModels.LineItem;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class OrderControllerTests : WebFixtures
{
    private readonly VerifySettings verifySettings;

    public OrderControllerTests()
    {
        this.verifySettings = new();
        this.verifySettings.ScrubMembers("traceId");
    }

    #region Get
    [Fact]
    public async Task<VerifyResult> GetOrders_ReturnsList()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/orders");
            _.StatusCodeShouldBeOk();
        });

        var orders = result.ReadAsJson<IEnumerable<Order>>();
        return await Verify(orders, this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> GetOrder_OrderExists_ReturnsOrder()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/orders/1");
            _.StatusCodeShouldBeOk();
        });

        var order = result.ReadAsJson<Order>();
        return await Verify(order, this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> GetOrder_OrderDoesNotExist_Returns404()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/orders/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
    }
    #endregion

    #region Post
    [Fact]
    public async Task<VerifyResult> PostOrder_ValidModel_ReturnsNewOrder()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    lineItems = new List<RMLineItems>()
                    {
                        new RMLineItems() { ProductId = 1, Quantity = 1 },
                    },
                })
                .ToUrl("/api/orders");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var order = result.ReadAsJson<Order>();
        return await Verify(order, this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PostOrder_InValidDTO_Fails()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    lineItems = new List<RMLineItems>()
                    {
                        new RMLineItems() { ProductId = 1, Quantity = -1 },
                    },
                })
                .ToUrl("/api/orders");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
    }
    #endregion

    #region Put
    [Fact]
    public async Task<VerifyResult> PutOrder_ValidDTO_ReturnsModel()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    lineItems = new List<RMLineItems>()
                    {
                        new RMLineItems() { ProductId = 1, Quantity = 1 },
                    },
                })
                .ToUrl("/api/orders");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var order = postResult.ReadAsJson<Order>();

        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    lineItems = new List<RMLineItems>()
                    {
                        new RMLineItems() { ProductId = 1, Quantity = 2 },
                    },
                })
                .ToUrl($"/api/orders/{order?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var updatedOrder = putResult.ReadAsJson<Order>();
        return await Verify(updatedOrder, this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutOrder_InvalidDTO_ReturnsBadRequest()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    lineItems = new List<RMLineItems>()
                    {
                        new RMLineItems() { ProductId = 1, Quantity = -1 },
                    },
                })
                .ToUrl("/api/orders/1");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutOrder_InvalidId_ReturnsNotFound()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    lineItems = new List<RMLineItems>()
                    {
                        new RMLineItems() { ProductId = 1, Quantity = 1 },
                    },
                })
                .ToUrl("/api/orders/5");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }
    #endregion

    #region Retire
    [Fact]
    public async Task<VerifyResult> CancelOrder_InvalidId_ReturnsNotFound()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url("/api/orders/5");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task CancelOrder_ValidId_ReturnsNoContent()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    lineItems = new List<RMLineItems>()
                    {
                        new RMLineItems() { ProductId = 1, Quantity = 1 },
                    },
                })
                .ToUrl("/api/orders");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var order = postResult.ReadAsJson<Order>();

        await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/orders/{order?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NoContent);
        });
    }

    [Fact]
    public async Task<VerifyResult> CancelOrder_ValidId_IsCanceled_ReturnsBadRequest()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    lineItems = new List<RMLineItems>()
                    {
                        new RMLineItems() { ProductId = 1, Quantity = 1 },
                    },
                })
                .ToUrl("/api/orders");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var order = postResult.ReadAsJson<Order>();

        await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/orders/{order?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NoContent);
        });

        var secondDeleteResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/orders/{order?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(secondDeleteResult.ReadAsTextAsync(), this.verifySettings);
    }
    #endregion
}
