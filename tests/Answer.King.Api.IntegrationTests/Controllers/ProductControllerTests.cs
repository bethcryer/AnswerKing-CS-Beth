using Alba;
using Answer.King.Api.IntegrationTests.Common;
using Answer.King.Api.RequestModels;
using Product = Answer.King.Api.IntegrationTests.Common.Models.Product;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class ProductControllerTests : IClassFixture<WebFixtures>
{
    private readonly IAlbaHost _host;

    private VerifySettings _errorLevelSettings;

    public ProductControllerTests(WebFixtures app)
    {
        this._host = app.AlbaHost;

        this._errorLevelSettings = new();
        this._errorLevelSettings.ScrubMember("traceId");
    }

    #region Get
    [Fact]
    public async Task<VerifyResult> GetProducts_ReturnsList()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Get.Url("/api/products");
            _.StatusCodeShouldBeOk();
        });

        var products = result.ReadAsJson<IEnumerable<Product>>();
        return await Verify(products);
    }

    [Fact]
    public async Task<VerifyResult> GetProduct_ProductExists_ReturnsProduct()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Get.Url("/api/products/1");
            _.StatusCodeShouldBeOk();
        });

        var products = result.ReadAsJson<Product>();
        return await Verify(products);
    }

    [Fact]
    public async Task<VerifyResult> GetProduct_ProductDoesNotExist_Returns404()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Get.Url("/api/products/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion

    #region Post
    [Fact]
    public async Task<VerifyResult> PostProduct_ValidModel_ReturnsNewProduct()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var products = result.ReadAsJson<Product>();
        return await Verify(products);
    }

    [Fact]
    public async Task<VerifyResult> PostProduct_InValidDTO_Fails()
    {
        var result = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 4 }
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion

    #region Put
    [Fact]
    public async Task<VerifyResult> PutProduct_ValidDTO_ReturnsModel()
    {
        var postResult = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var products = postResult.ReadAsJson<Product>();

        var putResult = await this._host.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "BBQ Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl($"/api/products/{products?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var updatedProduct = putResult.ReadAsJson<Product>();
        return await Verify(updatedProduct);
    }

    [Fact]
    public async Task<VerifyResult> PutProduct_InvalidDTO_ReturnsBadRequest()
    {
        var putResult = await this._host.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "BBQ Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 4 }
                })
                .ToUrl("/api/products/1");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }

    [Fact]
    public async Task<VerifyResult> PutProduct_InvalidId_ReturnsNotFound()
    {
        var putResult = await this._host.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "BBQ Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl("/api/products/5");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion

    #region Retire
    [Fact]
    public async Task<VerifyResult> RetireProduct_InvalidId_ReturnsNotFound()
    {
        var putResult = await this._host.Scenario(_ =>
        {
            _.Delete
                .Url("/api/products/5");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }

    [Fact]
    public async Task<VerifyResult> RetireProduct_ValidId_ReturnsOk()
    {
        var postResult = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var products = postResult.ReadAsJson<Product>();

        var putResult = await this._host.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/products/{products?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._errorLevelSettings);
    }

    [Fact]
    public async Task<VerifyResult> RetireProduct_ValidId_IsRetired_ReturnsNotFound()
    {
        var postResult = await this._host.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 }
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var products = postResult.ReadAsJson<Product>();

        await this._host.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/products/{products?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var secondDeleteResult = await this._host.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/products/{products?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Gone);
        });

        return await VerifyJson(secondDeleteResult.ReadAsTextAsync(), this._errorLevelSettings);
    }
    #endregion
}
