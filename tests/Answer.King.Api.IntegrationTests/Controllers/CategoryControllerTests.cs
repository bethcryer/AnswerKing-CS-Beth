using Alba;
using Answer.King.Api.IntegrationTests.Common;
using Answer.King.Api.RequestModels;
using VerifyTests;
using VerifyXunit;
using Xunit;
using Category = Answer.King.Api.IntegrationTests.Common.Models.Category;

namespace Answer.King.Api.IntegrationTests.Controllers;


[UsesVerify]
public class CategoryControllerTests : WebFixtures
{
    private readonly VerifySettings _verifySettings;

    public CategoryControllerTests()
    {
        this._verifySettings = new();
        this._verifySettings.ScrubMembers("traceId");
    }

    #region Get
    [Fact]
    public async Task<VerifyResult> GetCategories_ReturnsList()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/categories");
            _.StatusCodeShouldBeOk();
        });

        var products = result.ReadAsJson<IEnumerable<Category>>();
        return await Verify(products);
    }

    [Fact]
    public async Task<VerifyResult> GetCategory_CategoryExists_ReturnsCategory()
    {
        var category = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans",
                    Products = new List<long> { }
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/categories/1");
            _.StatusCodeShouldBeOk();
        });

        var products = result.ReadAsJson<Category>();
        return await Verify(products);
    }

    [Fact]
    public async Task<VerifyResult> GetCategory_CategoryDoesNotExist_Returns404()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/categories/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._verifySettings);
    }
    #endregion

    #region Post
    [Fact]
    public async Task<VerifyResult> PostCategory_ValidModel_ReturnsNewCategory()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans",
                    Products = new List<long> { }
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var products = result.ReadAsJson<Category>();
        return await Verify(products);
    }

    [Fact]
    public async Task<VerifyResult> PostCategory_InvalidDTO_ReturnsBadRequest()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood"
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PostCategory_InvalidProductId_ReturnsBadRequest()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans",
                    Products = new List<long> { 5 }
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._verifySettings);
    }
    #endregion

    #region Put
    [Fact]
    public async Task<VerifyResult> PutCategory_ValidDTO_ReturnsModel()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans",
                    Products = new List<long>()
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var category = postResult.ReadAsJson<Category>();

        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans and the high seas and also the puddles maybe",
                    Products = new List<long>()
                })
                .ToUrl($"/api/categories/{category?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var updatedCategory = putResult.ReadAsJson<Category>();
        return await Verify(updatedCategory);
    }

    [Fact]
    public async Task<VerifyResult> PutCategory_InvalidDTO_ReturnsBadRequest()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Seafood"
                })
                .ToUrl("/api/categories/1");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutCategory_InvalidId_ReturnsNotFound()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans",
                    Products = new List<long> { }
                })
                .ToUrl("/api/categories/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutCategory_InvalidProductId_ReturnsBadRequest()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans",
                    Products = new List<long> { 5 }
                })
                .ToUrl("/api/categories/1");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._verifySettings);
    }
    #endregion

    #region Retire
    [Fact]
    public async Task<VerifyResult> RetireCategory_InvalidId_ReturnsNotFound()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url("/api/categories/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> RetireCategory_ValidId_ReturnsOk()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans",
                    Products = new List<long> { }
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var categories = postResult.ReadAsJson<Category>();

        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/categories/{categories?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> RetireCategory_ValidId_IsRetired_ReturnsNotFound()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans",
                    Products = new List<long> { }
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var categories = postResult.ReadAsJson<Category>();

        await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/categories/{categories?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var secondDeleteResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/categories/{categories?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Gone);
        });

        return await VerifyJson(secondDeleteResult.ReadAsTextAsync(), this._verifySettings);
    }
    #endregion
}
