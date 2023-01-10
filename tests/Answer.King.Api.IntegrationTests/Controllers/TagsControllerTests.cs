using Alba;
using Answer.King.Api.IntegrationTests.Common;
using Tag = Answer.King.Api.IntegrationTests.Common.Models.Tag;
using Product = Answer.King.Api.IntegrationTests.Common.Models.Product;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class TagsControllerTests : WebFixtures
{
    private readonly VerifySettings _verifySettings;

    public TagsControllerTests()
    {
        this._verifySettings = new();
        this._verifySettings.ScrubMembers("traceId");
    }

    #region Get
    [Fact]
    public async Task<VerifyResult> GetTags_ReturnsList()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/tags");
            _.StatusCodeShouldBeOk();
        });

        var tags = result.ReadAsJson<IEnumerable<Tag>>();
        return await Verify(tags);
    }

    [Fact]
    public async Task<VerifyResult> GetTag_TagExists_ReturnsTag()
    {
        var newTag = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Non-animal products"
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/tags/1");
            _.StatusCodeShouldBeOk();
        });

        var tag = result.ReadAsJson<Tag>();
        return await Verify(tag);
    }

    [Fact]
    public async Task<VerifyResult> GetTag_TagDoesNotExist_Returns404()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/tags/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._verifySettings);
    }
    #endregion

    #region Get Products

    [Fact]
    public async Task<VerifyResult> GetProducts_TagExists_ReturnsProducts()
    {
        var newTag = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Non-animal products"
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/tags/1/products");
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
            _.Get.Url("/api/tags/50/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._verifySettings);
    }
    #endregion

    #region Post
    [Fact]
    public async Task<VerifyResult> PostTag_ValidModel_ReturnsNewTag()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Non-animal products"
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var tag = result.ReadAsJson<Tag>();
        return await Verify(tag);
    }

    [Fact]
    public async Task<VerifyResult> PostTag_InvalidDTO_ReturnsBadRequest()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Vegan"
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this._verifySettings);
    }
    #endregion

    #region Put
    [Fact]
    public async Task<VerifyResult> PutTag_ValidDTO_ReturnsModel()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Non-animal products"
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var tag = postResult.ReadAsJson<Tag>();

        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Edited Non-animal products"
                })
                .ToUrl($"/api/tags/{tag?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var updatedTag = putResult.ReadAsJson<Tag>();
        return await Verify(updatedTag);
    }

    [Fact]
    public async Task<VerifyResult> PutTag_InvalidDTO_ReturnsBadRequest()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Vegan"
                })
                .ToUrl("/api/tags/1");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutTag_InvalidId_ReturnsNotFound()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Edited Non-animal products"
                })
                .ToUrl("/api/tags/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._verifySettings);
    }
    #endregion

    #region Put: Add Product
    [Fact]
    public async Task<VerifyResult> PutAddProducts_ValidDTO_ReturnsModel()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Non-animal products"
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var tag = postResult.ReadAsJson<Tag>();

        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Products = new List<long>()
                })
                .ToUrl($"/api/tags/{tag?.Id}/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var updatedTag = putResult.ReadAsJson<Tag>();
        return await Verify(updatedTag);
    }

    [Fact]
    public async Task<VerifyResult> PutAddProducts_InvalidDTO_ReturnsBadRequest()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Products = 1
                })
                .ToUrl("/api/tags/1/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutAddProducts_InvalidId_ReturnsNotFound()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Products = new List<long>()
                })
                .ToUrl("/api/tags/50/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._verifySettings);
    }
    #endregion

    #region Delete: Remove Product
    [Fact]
    public async Task<VerifyResult> DeleteRemoveProducts_ValidDTO_ReturnsModel()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Non-animal products"
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var tag = postResult.ReadAsJson<Tag>();

        var deleteResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Json(new
                {
                    Products = new List<long>()
                })
                .ToUrl($"/api/tags/{tag?.Id}/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var updatedTag = deleteResult.ReadAsJson<Tag>();
        return await Verify(updatedTag);
    }

    [Fact]
    public async Task<VerifyResult> DeleteRemoveProducts_InvalidDTO_ReturnsBadRequest()
    {
        var deleteResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Json(new
                {
                    Products = 1
                })
                .ToUrl("/api/tags/1/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(deleteResult.ReadAsTextAsync(), this._verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> DeleteRemoveProducts_InvalidId_ReturnsNotFound()
    {
        var deleteResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Json(new
                {
                    Products = new List<long>()
                })
                .ToUrl("/api/tags/50/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(deleteResult.ReadAsTextAsync(), this._verifySettings);
    }
    #endregion

    #region Retire
    [Fact]
    public async Task<VerifyResult> RetireTag_InvalidId_ReturnsNotFound()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url("/api/tags/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this._verifySettings);
    }

    [Fact]
    public async Task RetireTag_ValidId_ReturnsNoContent()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Non-animal products"
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var tag = postResult.ReadAsJson<Tag>();

        await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/tags/{tag?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NoContent);
        });
    }

    [Fact]
    public async Task<VerifyResult> RetireTag_ValidId_IsRetired_ReturnsNotFound()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Non-animal products"
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var tag = postResult.ReadAsJson<Tag>();

        await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/tags/{tag?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NoContent);
        });

        var secondDeleteResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Delete
                .Url($"/api/tags/{tag?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Gone);
        });

        return await VerifyJson(secondDeleteResult.ReadAsTextAsync(), this._verifySettings);
    }
    #endregion
}
