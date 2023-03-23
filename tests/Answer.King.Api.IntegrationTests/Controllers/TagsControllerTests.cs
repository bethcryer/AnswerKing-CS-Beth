using Answer.King.Api.IntegrationTests.Common;
using Answer.King.Api.RequestModels;
using Product = Answer.King.Api.IntegrationTests.Common.Models.Product;
using Tag = Answer.King.Api.IntegrationTests.Common.Models.Tag;

namespace Answer.King.Api.IntegrationTests.Controllers;

[UsesVerify]
public class TagsControllerTests : WebFixtures
{
    private readonly VerifySettings verifySettings;

    public TagsControllerTests()
    {
        this.verifySettings = new();
        this.verifySettings.ScrubMembers("traceId");
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
        await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Test Tag",
                    Description = "Non-animal products",
                    Products = new List<long>(),
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

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
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
                    Name = "Test Tag",
                    Description = "Non-animal products",
                    Products = new List<long>(),
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var tag = result.ReadAsJson<Tag>();
        return await Verify(tag);
    }

    [Fact]
    public async Task<VerifyResult> PostTag_ValidModelWithProducts_ReturnsNewTag()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Test Tag",
                    Description = "Non-animal products",
                    Products = new List<long> { 1 }, // valid product ID
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var tag = result.ReadAsJson<Tag>();
        return await Verify(tag);
    }

    [Fact]
    public async Task<VerifyResult> PostTag_ValidModelWithRetiredProducts_ReturnsBadRequest_DoesNotPartiallyCreateTag()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Test Tag",
                    Description = "Non-animal products",
                    Products = new List<long> { 3 }, // retired product ID
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        var allTagsResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Get.Url("/api/tags");
            _.StatusCodeShouldBeOk();
        });

        var allTags = allTagsResult.ReadAsJson<IEnumerable<Tag>>();

        Assert.All(allTags!, tag => Assert.NotEqual("Test Tag", tag.Name));

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PostTag_InvalidDTO_ReturnsBadRequest()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Test Tag",
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PostTag_DuplicateName_ReturnsBadRequest()
    {
        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Non-Animal Products",
                    Products = new List<long>(),
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(result.ReadAsTextAsync(), this.verifySettings);
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
                    Name = "Test Tag",
                    Description = "Non-animal products",
                    Products = new List<long>(),
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var tag = postResult.ReadAsJson<Tag>();

        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Test Tag",
                    Description = "Edited Non-animal products",
                    Products = new List<long>(),
                })
                .ToUrl($"/api/tags/{tag?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var updatedTag = result.ReadAsJson<Tag>();
        return await Verify(updatedTag);
    }

    [Fact]
    public async Task<VerifyResult> PutTag_ValidDTOWithProducts_ReturnsModel()
    {
        var productResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Burger",
                    Description = "Juicy",
                    Price = 1.50,
                    CategoryId = new CategoryId(1),
                    Tags = Array.Empty<long>(),
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var product = productResult.ReadAsJson<Product>();

        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Test Tag",
                    Description = "Non-animal products",
                    Products = new List<long>(),
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.Created);
        });

        var tag = postResult.ReadAsJson<Tag>();

        var result = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Test Tag",
                    Description = "Edited Non-animal products",
                    Products = new List<long> { product!.Id },
                })
                .ToUrl($"/api/tags/{tag?.Id}");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.OK);
        });

        var updatedTag = result.ReadAsJson<Tag>();
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
                    Name = "Test Tag",
                    Products = new List<long>(),
                })
                .ToUrl("/api/tags/1");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutTag_InvalidId_ReturnsNotFound()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Test Tag",
                    Description = "Edited Non-animal products",
                    Products = new List<long>(),
                })
                .ToUrl("/api/tags/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task<VerifyResult> PutTag_DuplicateName_ReturnsBadRequest()
    {
        var putResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Put
                .Json(new
                {
                    Name = "Vegan",
                    Description = "Edited Non-animal products",
                    Products = new List<long>(),
                })
                .ToUrl("/api/tags/2");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.BadRequest);
        });

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
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

        return await VerifyJson(putResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task RetireTag_ValidId_ReturnsNoContent()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Test Tag",
                    Description = "Non-animal products",
                    Products = new List<long>(),
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
                    Name = "Test Tag",
                    Description = "Non-animal products",
                    Products = new List<long>(),
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

        return await VerifyJson(secondDeleteResult.ReadAsTextAsync(), this.verifySettings);
    }
    #endregion

    #region Unretire
    [Fact]
    public async Task<VerifyResult> UnretireTag_InvalidId_ReturnsNotFound()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Url("/api/tags/50");
            _.StatusCodeShouldBe(System.Net.HttpStatusCode.NotFound);
        });

        return await VerifyJson(postResult.ReadAsTextAsync(), this.verifySettings);
    }

    [Fact]
    public async Task UnretireTag_ValidId_ReturnsNoContent()
    {
        var postResult = await this.AlbaHost.Scenario(_ =>
        {
            _.Post
                .Json(new
                {
                    Name = "Test Tag",
                    Description = "Non-animal products",
                    Products = new List<long>(),
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
    #endregion
}
