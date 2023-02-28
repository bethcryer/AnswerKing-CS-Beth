using Answer.King.Api.IntegrationTests.Common;
using Answer.King.Api.RequestModels;
using Product = Answer.King.Api.IntegrationTests.Common.Models.Product;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class ProductsControllerTests : WebFixtures
{
    private readonly VerifySettings verifySettings;

    public ProductsControllerTests()
    {
        this.verifySettings = new();
        this.verifySettings.ScrubMembers("traceId");
    }

    #region Get
    [Fact]
    public async Task<VerifyResult> GetProducts_ReturnsList()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/products");
            _.StatusCodeShouldBeOk();
        });

        var products = result.ReadAsJson<IEnumerable<Product>>();
        return await Verify(products, this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> GetProduct_ProductExists_ReturnsProduct()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/products/1");
            _.StatusCodeShouldBeOk();
        });

        var products = result.ReadAsJson<Product>();
        return await Verify(products, this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> GetProduct_ProductDoesNotExist_Returns404()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/products/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> GetProduct_UsingInvalidID_Returns400()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/products/invalidid");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
    }
    #endregion

    #region Post
    [Fact]
    public async Task<VerifyResult> PostProduct_ValidModel_ReturnsNewProduct()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    CategoryId = new CategoryId(1),
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var products = result.ReadAsJson<Product>();
        return await Verify(products, this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PostProduct_InValidDTO_Fails()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PostProduct_InValidJSONFormat_Fails()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
            .Text("InvalidJSON")
            .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.UnsupportedMediaType);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PostProduct_DuplicateName_ReturnsBadRequest()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Fish",
                    Description = "Juicy",
                    Price = 1.50,
                    CategoryId = new CategoryId(1),
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
    }
    #endregion

    #region Put
    [Fact]
    public async Task<VerifyResult> PutProduct_ValidDTO_ReturnsModel()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    CategoryId = new CategoryId(1),
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var products = postResult.ReadAsJson<Product>();

        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "BBQ Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    CategoryId = new CategoryId(2),
                })
                .ToUrl($"/api/products/{products?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var updatedProduct = putResult.ReadAsJson<Product>();
        return await Verify(updatedProduct, this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutProduct_InvalidDTO_ReturnsBadRequest()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "BBQ Burger",
                })
                .ToUrl("/api/products/1");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutProduct_NonExistentID_ReturnsNotFound()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "BBQ Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    CategoryId = new CategoryId(1),
                })
                .ToUrl("/api/products/1000");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutProduct_InvalidID_ReturnsBadRequest()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "BBQ Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    Categories = new List<long> { 1 },
                })
                .ToUrl("/api/products/InvalidID");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutProduct_InvalidJSON_Returns415()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Text("InvalidJSON")
                .ToUrl("/api/products/InvalidID");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.UnsupportedMediaType);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutProduct_RetiredProduct_ReturnsBadRequest()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "BBQ Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    CategoryId = 1,
                })
                .ToUrl("/api/products/3");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutProduct_DuplicateName_ReturnsBadRequest()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Fish",
                    Description = "Juicy",
                    Price = 1.50,
                    CategoryId = 1,
                })
                .ToUrl("/api/products/3");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    #endregion

    #region Retire
    [Fact]
    public async Task<VerifyResult> RetireProduct_InvalidId_ReturnsNotFound()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url("/api/products/1000");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task RetireProduct_ValidId_ReturnsNoContent()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.5,
                    CategoryId = new CategoryId(1),
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var products = postResult.ReadAsJson<Product>();

        await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/products/{products?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NoContent);
        });
    }

    [Fact]
    public async Task<VerifyResult> RetireProduct_ValidId_IsRetired_ReturnsNotFound()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    CategoryId = new CategoryId(1),
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var products = postResult.ReadAsJson<Product>();

        await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/products/{products?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NoContent);
        });

        var secondDeleteResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/products/{products?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Gone);
        });

        return await VerifyJson(secondDeleteResult.ReadAsTextAsync(), this.verifySettings);
    }
    #endregion

    #region Unretire
    [Fact]
    public async Task<VerifyResult> UnretireProduct_InvalidId_ReturnsNotFound()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Url("/api/products/1000");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(postResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> UnretireProduct_ValidId_ReturnsProduct()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.5,
                    CategoryId = new CategoryId(1),
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var products = postResult.ReadAsJson<Product>();

        await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/products/{products?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NoContent);
        });

        var unretireResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Url($"/api/products/{products?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        return await VerifyJson(unretireResult.ReadAsTextAsync(), this.verifySettings);
    }
    #endregion
}
