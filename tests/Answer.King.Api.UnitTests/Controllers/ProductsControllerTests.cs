using Answer.King.Api.Controllers;
using Answer.King.Api.Services;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Test.Common.CustomAsserts;
using Answer.King.Test.Common.CustomTraits;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using ProductRequest = Answer.King.Api.RequestModels.Product;

namespace Answer.King.Api.UnitTests.Controllers;

[TestCategory(TestType.Unit)]
public class ProductsControllerTests
{
    #region Setup

    private static readonly IProductService ProductService = Substitute.For<IProductService>();

    private static readonly ProductsController GetSubjectUnderTest = new(ProductService);

    #endregion Setup

    #region GenericControllerTests

    [Fact]
    public void Controller_RouteAttribute_IsPresentWithCorrectRoute()
    {
        // Assert
        AssertController.HasRouteAttribute<ProductsController>("api/[controller]");
        Assert.Equal("ProductsController", nameof(ProductsController));
    }

    #endregion GenericControllerTests

    #region GetAll

    [Fact]
    public void GetAll_Method_HasCorrectVerbAttribute()
    {
        // Assert
        AssertController.MethodHasVerb<ProductsController, HttpGetAttribute>(
            nameof(ProductsController.GetAll));
    }

    [Fact]
    public async Task GetAll_ValidRequest_ReturnsOkObjectResult()
    {
        // Act
        var result = await GetSubjectUnderTest.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    #endregion GetAll

    #region GetOne

    [Fact]
    public void GetOne_Method_HasCorrectVerbAttribute()
    {
        // Assert
        AssertController.MethodHasVerb<ProductsController, HttpGetAttribute>(
            nameof(ProductsController.GetOne));
    }

    [Fact]
    public async Task GetOne_ServiceReturnsNull_ReturnsNotFoundResult()
    {
        // Arrange
        const int id = 1;

        // Act
        var result = await GetSubjectUnderTest.GetOne(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetOne_ValidRequest_ReturnsOkObjectResult()
    {
        // Arrange
        const long id = 1;
        var products = new Product("name", "description", 1.99, new ProductCategory(1, "name", "description"));
        ProductService.GetProduct(Arg.Is(id)).Returns(products);

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
        AssertController.MethodHasVerb<ProductsController, HttpPostAttribute>(
            nameof(ProductsController.Post));
    }

    [Fact]
    public async Task Post_ValidRequestCallsGetAction_ReturnsNewProduct()
    {
        // Arrange
        var productRequestModel = new ProductRequest
        {
            Name = "PRODUCT_NAME",
            Description = "PRODUCT_DESCRIPTION",
            Price = 0,
        };

        var product = new Product("PRODUCT_NAME", "PRODUCT_DESCRIPTION", 0, new ProductCategory(1, "name", "description"));

        ProductService.CreateProduct(productRequestModel).Returns(product);

        // Act
        var result = await GetSubjectUnderTest.Post(productRequestModel);

        // Assert
        await ProductService.Received().CreateProduct(productRequestModel);
        Assert.IsType<CreatedAtActionResult>(result);
    }

    #endregion Post

    #region Put

    [Fact]
    public void Put_Method_HasCorrectVerbAttribute()
    {
        // Assert
        AssertController.MethodHasVerb<ProductsController, HttpPutAttribute>(
            nameof(ProductsController.Put), "{id}");
    }

    #endregion Put

    #region Retire

    [Fact]
    public void Retire_Method_HasCorrectVerbAttribute()
    {
        // Assert
        AssertController.MethodHasVerb<ProductsController, HttpDeleteAttribute>(
            nameof(ProductsController.Retire), "{id}");
    }

    #endregion Retire
}
