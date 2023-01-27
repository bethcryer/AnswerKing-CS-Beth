using Answer.King.Api.IntegrationTests.Common;
using Order = Answer.King.Api.IntegrationTests.Common.Models.Order;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class OrdersControllerUnseededTests : UnseededWebFixtures
{
    private readonly VerifySettings verifySettings;

    public OrdersControllerUnseededTests()
    {
        this.verifySettings = new();
        this.verifySettings.ScrubMembers("traceId");
    }

    #region Get
    [Fact]
    public async Task<VerifyResult> GetOrders_ReturnsEmptyList()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/orders");
            _.StatusCodeShouldBeOk();
        });

        var orders = result.ReadAsJson<IEnumerable<Order>>();
        return await Verify(orders, this.verifySettings);
    }
    #endregion
}
