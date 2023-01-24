using Answer.King.Api.Controllers;
using Answer.King.Api.Services;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Test.Common.CustomAsserts;
using Answer.King.Test.Common.CustomTraits;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using TagProductsRequest = Answer.King.Api.RequestModels.TagProducts;
using TagRequest = Answer.King.Api.RequestModels.Tag;

namespace Answer.King.Api.UnitTests.Controllers;

[TestCategory(TestType.Unit)]
public class TagsControllerTests
{
    #region Setup

    private static readonly ITagService TagService = Substitute.For<ITagService>();

    private static readonly IProductService ProductService = Substitute.For<IProductService>();

    private static readonly TagsController GetSubjectUnderTest = new(TagService, ProductService);

    #endregion Setup

    #region GenericControllerTests

    [Fact]
    public void Controller_RouteAttribute_IsPresentWithCorrectRoute()
    {
        // Assert
        AssertController.HasRouteAttribute<TagsController>("api/[controller]");
        Assert.Equal("TagsController", nameof(TagsController));
    }

    #endregion GenericControllerTests

    #region GetAll

    [Fact]
    public void GetAll_Method_HasCorrectVerbAttribute()
    {
        // Assert
        AssertController.MethodHasVerb<TagsController, HttpGetAttribute>(
            nameof(TagsController.GetAll));
    }

    [Fact]
    public async Task GetAll_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        TagService.GetTags().Returns(new List<Tag>());

        // Act
        var result = await GetSubjectUnderTest.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion GetAll

    #region GetOne

    [Fact]
    public void GetOne_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<TagsController, HttpGetAttribute>(
            nameof(TagsController.GetOne), "{id}");
    }

    [Fact]
    public async Task GetOne_ValidRequestWithNullResult_ReturnsNotFoundResult()
    {
        // Arrange
        Tag data = null!;
        TagService.GetTag(Arg.Any<long>()).Returns(data);

        // Act
        var result = await GetSubjectUnderTest.GetOne(Arg.Any<long>());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetOne_ValidRequestWithResult_ReturnsOkObjectResult()
    {
        // Arrange
        const long id = 1;
        var data = new Tag("name", "description", new List<ProductId>());
        TagService.GetTag(Arg.Is(id)).Returns(data);

        // Act
        var result = await GetSubjectUnderTest.GetOne(id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion GetOne

    #region Post

    [Fact]
    public void Post_Method_HasCorrectVerbAttribute()
    {
        // Assert
        AssertController.MethodHasVerb<TagsController, HttpPostAttribute>(
            nameof(TagsController.Post));
    }

    [Fact]
    public async Task Post_ValidRequestCallsGetAction_ReturnsNewTag()
    {
        // Arrange
        var tagRequestModel = new TagRequest
        {
            Name = "TAG_NAME",
            Description = "TAG_DESCRIPTION",
        };

        var tag = new Tag("TAG_NAME", "TAG_DESCRIPTION", new List<ProductId>());

        TagService.CreateTag(tagRequestModel).Returns(tag);

        // Act
        var result = await GetSubjectUnderTest.Post(tagRequestModel);

        // Assert
        await TagService.Received().CreateTag(tagRequestModel);
        Assert.IsType<CreatedAtActionResult>(result);
    }

    #endregion Post

    #region Put

    [Fact]
    public void Put_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<TagsController, HttpPutAttribute>(
            nameof(TagsController.Put), "{id}");
    }

    [Fact]
    public async Task Put_NullTag_ReturnsNotFoundResult()
    {
        // Arrange
        const int id = 1;

        // Act
        var result = await GetSubjectUnderTest.Put(id, null!);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Put_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        const int id = 1;
        var tagRequestModel = new TagRequest
        {
            Name = "TAG_NAME",
            Description = "TAG_DESCRIPTION",
        };

        var tag = new Tag("TAG_NAME", "TAG_DESCRIPTION", new List<ProductId>());

        TagService.UpdateTag(id, tagRequestModel).Returns(tag);

        // Act
        var result = await GetSubjectUnderTest.Put(id, tagRequestModel);

        // Assert
        Assert.Equal(tagRequestModel.Name, tag.Name);
        Assert.Equal(tagRequestModel.Description, tag.Description);
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion Put

    #region Put: Add Products

    [Fact]
    public void PutProducts_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<TagsController, HttpPutAttribute>(
            nameof(TagsController.AddProducts), "{id}/products");
    }

    [Fact]
    public async Task PutProducts_NullTag_ReturnsNotFoundResult()
    {
        // Arrange
        const int id = 1;

        // Act
        var result = await GetSubjectUnderTest.AddProducts(id, null!);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PutProducts_ProductIdNotValid_ReturnsValidationProblem()
    {
        // Arrange
        const int id = 1;
        var tagAddProductsRequestModel = new TagProductsRequest
        {
            Products = new List<long> { 1 },
        };

        TagService.AddProducts(id, tagAddProductsRequestModel).Throws(new TagServiceException("The provided product id is not valid."));

        // Act
        var result = await GetSubjectUnderTest.AddProducts(id, tagAddProductsRequestModel);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task PutProducts_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        const int id = 1;
        var tagAddProductsRequestModel = new TagProductsRequest
        {
            Products = new List<long> { 1 },
        };

        var tag = new Tag("TAG_NAME", "TAG_DESCRIPTION", new List<ProductId> { new(1) });

        TagService.AddProducts(id, tagAddProductsRequestModel).Returns(tag);

        // Act
        var result = await GetSubjectUnderTest.AddProducts(id, tagAddProductsRequestModel);

        // Assert
        Assert.Equal(tagAddProductsRequestModel.Products[0], tag.Products.First().Value);
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion Put

    #region Delete: Remove Products

    [Fact]
    public void DeleteProducts_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<TagsController, HttpDeleteAttribute>(
            nameof(TagsController.RemoveProducts), "{id}/products");
    }

    [Fact]
    public async Task DeleteProducts_NullTag_ReturnsNotFoundResult()
    {
        // Arrange
        const int id = 1;

        // Act
        var result = await GetSubjectUnderTest.RemoveProducts(id, null!);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteProducts_ProductIdNotValid_ReturnsValidationProblem()
    {
        // Arrange
        const int id = 1;
        var tagRemoveProductsRequestModel = new TagProductsRequest
        {
            Products = new List<long> { 1 },
        };

        TagService.RemoveProducts(id, tagRemoveProductsRequestModel).Throws(new TagServiceException("The provided product id is not valid."));

        // Act
        var result = await GetSubjectUnderTest.RemoveProducts(id, tagRemoveProductsRequestModel);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task DeleteProducts_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        const int id = 1;
        var tagAddProductsRequestModel = new TagProductsRequest
        {
            Products = new List<long> { 1 },
        };

        var tag = new Tag("TAG_NAME", "TAG_DESCRIPTION", new List<ProductId>());

        TagService.RemoveProducts(id, tagAddProductsRequestModel).Returns(tag);

        // Act
        var result = await GetSubjectUnderTest.RemoveProducts(id, tagAddProductsRequestModel);

        // Assert
        Assert.Empty(tag.Products);
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion Put

    #region Retire

    [Fact]
    public void Delete_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<TagsController, HttpDeleteAttribute>(
            nameof(TagsController.Retire), "{id}");
    }

    [Fact]
    public async Task Retire_NullTag_ReturnsNotFound()
    {
        // Arrange / Act
        var result = await GetSubjectUnderTest.Retire(Arg.Any<long>());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Retire_ProductsStillAssigned_ReturnsValidationProblem()
    {
        // Arrange
        const int id = 1;

        TagService.RetireTag(id).ThrowsAsync(new TagServiceException("Cannot retire tag whilst there are still products assigned."));

        // Act
        var result = await GetSubjectUnderTest.Retire(id);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public async Task Retire_ValidRequest_ReturnsNoContentResult()
    {
        // Arrange
        const int id = 1;
        var tag = new Tag("TAG_NAME", "TAG_DESCRIPTION", new List<ProductId>());

        TagService.RetireTag(id).Returns(tag);

        // Act
        var result = await GetSubjectUnderTest.Retire(id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    #endregion Retire

    #region GetProducts

    [Fact]
    public void GetProducts_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<TagsController, HttpGetAttribute>(
            nameof(TagsController.GetProducts), "{id}/products");
    }

    #endregion GetProducts
}
