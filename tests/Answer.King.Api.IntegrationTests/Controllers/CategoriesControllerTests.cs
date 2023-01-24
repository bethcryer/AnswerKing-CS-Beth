using Answer.King.Api.IntegrationTests.Common;
using Answer.King.Api.IntegrationTests.Common.Models;
using Category = Answer.King.Api.IntegrationTests.Common.Models.Category;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class CategoriesControllerTests : WebFixtures
{
    private readonly VerifySettings verifySettings;

    public CategoriesControllerTests()
    {
        this.verifySettings = new();
        this.verifySettings.ScrubMembers("traceId");
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

        var categories = result.ReadAsJson<IEnumerable<Category>>();
        return await Verify(categories);
    }

    [Fact]
    public async Task<VerifyResult> GetCategory_CategoryExists_ReturnsCategory()
    {
        await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans",
                    Products = new List<long>(),
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/categories/1");
            _.StatusCodeShouldBeOk();
        });

        var category = result.ReadAsJson<Category>();
        return await Verify(category);
    }

    [Fact]
    public async Task<VerifyResult> GetCategory_CategoryDoesNotExist_Returns404()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/categories/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
    }
    #endregion

    #region Get Products

    [Fact]
    public async Task<VerifyResult> GetProducts_TagExists_ReturnsProducts()
    {
        await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans",
                    Products = new List<long>(),
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/categories/1/products");
            _.StatusCodeShouldBeOk();
        });

        var tag = result.ReadAsJson<IEnumerable<Product>>();
        return await Verify(tag);
    }

    [Fact]
    public async Task<VerifyResult> GetProducts_TagDoesNotExist_Returns404()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/categories/50/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
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
                    Products = new List<long>(),
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var category = result.ReadAsJson<Category>();
        return await Verify(category);
    }

    [Fact]
    public async Task<VerifyResult> PostCategory_InvalidDTO_ReturnsBadRequest()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
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
                    Products = new List<long> { 5 },
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
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
                    Products = new List<long>(),
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
                    Products = new List<long>(),
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
                    Name = "Seafood",
                })
                .ToUrl("/api/categories/1");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
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
                    Products = new List<long>(),
                })
                .ToUrl("/api/categories/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
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
                    Products = new List<long> { 5 },
                })
                .ToUrl("/api/categories/1");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
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

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task RetireCategory_ValidId_ReturnsNoContent()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Seafood",
                    Description = "Food from the oceans",
                    Products = new List<long>(),
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var category = postResult.ReadAsJson<Category>();

        await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/categories/{category?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NoContent);
        });
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
                    Products = new List<long>(),
                })
                .ToUrl("/api/categories");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var category = postResult.ReadAsJson<Category>();

        await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/categories/{category?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NoContent);
        });

        var secondDeleteResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/categories/{category?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Gone);
        });

        return await VerifyJson(secondDeleteResult.ReadAsTextAsync(), this.verifySettings);
    }
    #endregion
}
