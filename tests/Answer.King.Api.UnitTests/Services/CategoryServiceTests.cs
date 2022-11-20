using Answer.King.Api.Services;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;
using Xunit;

namespace Answer.King.Api.UnitTests.Services;

[TestCategory(TestType.Unit)]
public class CategoryServiceTests
{
    #region Retire

    [Fact]
    public async void RetireCategory_InvalidCategoryIdReceived_ReturnsNull()
    {
        // Arrange
        this.CategoryRepository.Get(Arg.Any<long>()).Returns(null as Category);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        Assert.Null(await sut.RetireCategory(1));
    }

    [Fact]
    public async void RetireCategory_CategoryContainsProducts_ThrowsException()
    {
        // Arrange
        var category = new Category("category", "desc");
        category.AddProduct(new ProductId(1));

        this.CategoryRepository.Get(category.Id).Returns(category);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<CategoryServiceException>(() =>
            sut.RetireCategory(category.Id));
    }

    [Fact]
    public async void RetireCategory_NoProductsAssociatedWithCategory_ReturnsRetiredCategory()
    {
        // Arrange
        var category = new Category("category", "desc");
        this.CategoryRepository.Get(category.Id).Returns(category);

        // Act
        var sut = this.GetServiceUnderTest();
        var retiredCategory = await sut.RetireCategory(category.Id);

        // Assert
        Assert.True(retiredCategory!.Retired);
    }

    #endregion

    #region Create

    [Fact]
    public async void CreateCategory_ValidCategory_ReturnsNewlyCreatedCategory()
    {
        // Arrange
        var products = new List<long> { 1 };
        var request = new RequestModels.Category
        {
            Name = "category",
            Description = "desc",
            Products = products
        };

        // Act
        var sut = this.GetServiceUnderTest();
        var category = await sut.CreateCategory(request);

        // Assert
        Assert.Equal(request.Name, category.Name);
        Assert.Equal(request.Description, category.Description);

        await this.CategoryRepository.Received().Save(Arg.Any<Category>());
    }

    #endregion

    #region Get

    [Fact]
    public async void GetCategory_ValdidCategoryId_ReturnsCategory()
    {
        // Arrange
        var category = new Category("category", "desc");
        var id = category.Id;

        this.CategoryRepository.Get(id).Returns(category);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualCategory = await sut.GetCategory(id);

        // Assert
        Assert.Equal(category, actualCategory);
        await this.CategoryRepository.Received().Get(id);
    }

    [Fact]
    public async void GetCategories_ReturnsAllCategories()
    {
        // Arrange
        var categories = new[]
        {
            new Category("category 1", "desc"),
            new Category("category 2", "desc")
        };

        this.CategoryRepository.Get().Returns(categories);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualCategories = await sut.GetCategories();

        // Assert
        Assert.Equal(categories, actualCategories);
        await this.CategoryRepository.Received().Get();
    }

    #endregion

    #region Update

    [Fact]
    public async void UpdateCategory_InvalidCategoryId_ReturnsNull()
    {
        // Arrange
        var updateCategoryRequest = new RequestModels.Category();
        var categoryId = 1;

        // Act
        var sut = this.GetServiceUnderTest();
        var category = await sut.UpdateCategory(categoryId, updateCategoryRequest);

        // Assert
        Assert.Null(category);
    }

    [Fact]
    public async void UpdateCategory_ValidCategoryIdAndRequest_ReturnsUpdatedCategory()
    {
        // Arrange
        var oldCategory = new Category("old category", "old desc");
        var categoryId = oldCategory.Id;

        var updateCategoryRequest = new RequestModels.Category
        {
            Name = "updated category",
            Description = "updated desc"
        };

        this.CategoryRepository.Get(categoryId).Returns(oldCategory);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualCategory = await sut.UpdateCategory(categoryId, updateCategoryRequest);

        // Assert
        Assert.Equal(updateCategoryRequest.Name, actualCategory!.Name);
        Assert.Equal(updateCategoryRequest.Description, actualCategory.Description);

        await this.CategoryRepository.Received().Get(categoryId);
        await this.CategoryRepository.Received().Save(Arg.Any<Category>());
    }

    #endregion

    #region Setup

    private readonly ICategoryRepository CategoryRepository = Substitute.For<ICategoryRepository>();

    private ICategoryService GetServiceUnderTest()
    {
        return new CategoryService(this.CategoryRepository);
    }

    #endregion
}
