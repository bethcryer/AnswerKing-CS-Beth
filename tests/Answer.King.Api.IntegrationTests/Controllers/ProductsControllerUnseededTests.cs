using Answer.King.Api.IntegrationTests.Common;
using Product = Answer.King.Api.IntegrationTests.Common.Models.Product;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class ProductsControllerUnseededTests : UnseededWebFixtures
{
    private readonly VerifySettings verifySettings;

    public ProductsControllerUnseededTests()
    {
        this.verifySettings = new();
        this.verifySettings.ScrubMembers("traceId");
    }

    #region Get
    [Fact]
    public async Task<VerifyResult> GetProducts_ReturnsEmptyList()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/products");
            _.StatusCodeShouldBeOk();
        });

        var products = result.ReadAsJson<IEnumerable<Product>>();
        return await Verify(products, this.verifySettings);
    }
    #endregion
}
