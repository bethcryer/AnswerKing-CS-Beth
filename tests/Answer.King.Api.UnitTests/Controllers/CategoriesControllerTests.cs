using Answer.King.Api.Controllers;
using Answer.King.Api.Services;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Test.Common.CustomAsserts;
using Answer.King.Test.Common.CustomTraits;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Answer.King.Api.UnitTests.Controllers;

[TestCategory(TestType.Unit)]
public class CategoriesControllerTests
{
    #region GenericControllerTests

    [Fact]
    public void Controller_RouteAttribute_IsPresentWithCorrectRoute()
    {
        // Assert
        AssertController.HasRouteAttribute<CategoriesController>("api/[controller]");
        Assert.Equal(nameof(CategoriesController), "CategoriesController");
    }

    #endregion GenericControllerTests

    #region GetAll

    [Fact]
    public void GetAll_Method_HasCorrectVerbAttribute()
    {
        // Assert
        AssertController.MethodHasVerb<CategoriesController, HttpGetAttribute>(
            nameof(CategoriesController.GetAll));
    }

    [Fact]
    public async void GetAll_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        var data = new List<Category>();
        CategoryService.GetCategories().Returns(data);

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
        AssertController.MethodHasVerb<CategoriesController, HttpGetAttribute>(
            nameof(CategoriesController.GetOne), "{id}");
    }

    [Fact]
    public async Task GetOne_ValidRequestWithNullResult_ReturnsNotFoundResult()
    {
        // Arrange
        Category data = null!;
        CategoryService.GetCategory(Arg.Any<long>()).Returns(data);

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
        var data = new Category("name", "description", new List<ProductId>());
        CategoryService.GetCategory(Arg.Is(id)).Returns(data);

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
        AssertController.MethodHasVerb<CategoriesController, HttpPostAttribute>(
            nameof(CategoriesController.Post));
    }

    [Fact(Skip = "This test needs to be written, but not sure due best way due to Domain.Inventory.Category protection level")]
    public void Post_ValidRequestCallsGetAction_ReturnsNewCategory()
    {
        // Arrange

        // Act

        // Assert
    }

    #endregion Post

    #region Put

    [Fact]
    public void Put_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<CategoriesController, HttpPutAttribute>(
            nameof(CategoriesController.Put), "{id}");
    }

    [Fact]
    public async void Put_NullCategory_ReturnsNotFoundResult()
    {
        // Arrange
        const int id = 1;

        // Act
        var result = await GetSubjectUnderTest.Put(id, null!);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void Put_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        const int id = 1;
        var categoryRequestModel = new RequestModels.Category
        {
            Name = "CATEGORY_NAME",
            Description = "CATEGORY_DESCRIPTION"
        };

        var category = new Category("CATEGORY_NAME", "CATEGORY_DESCRIPTION", new List<ProductId>());

        CategoryService.UpdateCategory(id, categoryRequestModel).Returns(category);

        // Act
        var result = await GetSubjectUnderTest.Put(id, categoryRequestModel);

        // Assert
        Assert.Equal(categoryRequestModel.Name, category.Name);
        Assert.Equal(categoryRequestModel.Description, category.Description);
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion Put

    #region Retire

    [Fact]
    public void Delete_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<CategoriesController, HttpDeleteAttribute>(
            nameof(CategoriesController.Retire), "{id}");
    }

    [Fact]
    public async void Retire_NullCategory_ReturnsNotFound()
    {
        // Arrange / Act
        var result = await GetSubjectUnderTest.Retire(Arg.Any<long>());

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async void Retire_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        const int id = 1;
        var category = new Category("CATEGORY_NAME", "CATEGORY_DESCRIPTION", new List<ProductId>());

        CategoryService.RetireCategory(id).Returns(category);

        // Act
        var result = await GetSubjectUnderTest.Retire(id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion Retire

    #region GetProducts

    [Fact]
    public void GetProducts_Method_HasCorrectVerbAttributeAndPath()
    {
        // Assert
        AssertController.MethodHasVerb<CategoriesController, HttpGetAttribute>(
            nameof(CategoriesController.GetProducts), "{id}/products");
    }

    #endregion GetProducts

    #region Setup

    private static readonly ICategoryService CategoryService = Substitute.For<ICategoryService>();

    private static readonly IProductService ProductService = Substitute.For<IProductService>();

    private static readonly CategoriesController GetSubjectUnderTest =
        new CategoriesController(CategoryService, ProductService);

    #endregion Setup
}
