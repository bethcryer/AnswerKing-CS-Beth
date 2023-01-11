using Answer.King.Api.Services;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;
using Xunit;

namespace Answer.King.Api.UnitTests.Services;

[TestCategory(TestType.Unit)]
public class CategoryServiceTests
{
    private static readonly CategoryFactory categoryFactory = new();

    private static readonly ProductFactory productFactory = new();

    #region Retire

    [Fact]
    public async Task RetireCategory_InvalidCategoryIdReceived_ReturnsNull()
    {
        // Arrange
        this.CategoryRepository.Get(Arg.Any<long>()).Returns(null as Category);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        Assert.Null(await sut.RetireCategory(1));
    }

    [Fact]
    public async Task RetireCategory_CategoryContainsProducts_ThrowsException()
    {
        // Arrange
        var category = new Category("category", "desc", new List<ProductId>());
        category.AddProduct(new ProductId(1));

        this.CategoryRepository.Get(category.Id).Returns(category);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<CategoryServiceException>(() =>
            sut.RetireCategory(category.Id));
    }

    [Fact]
    public async Task RetireCategory_AlreadyRetired_ThrowsException()
    {
        // Arrange
        var category = new Category("category", "desc", new List<ProductId>());
        category.RetireCategory();
        this.CategoryRepository.Get(category.Id).Returns(category);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<CategoryServiceException>(() =>
            sut.RetireCategory(category.Id));
    }

    [Fact]
    public async Task RetireCategory_NoProductsAssociatedWithCategory_ReturnsRetiredCategory()
    {
        // Arrange
        var category = new Category("category", "desc", new List<ProductId>());
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
    public async Task CreateCategory_InvalidProductIdInCategory_ThrowsException()
    {
        // Arrange
        var categoryRequest = new RequestModels.Category
        {
            Name = "Laptop",
            Description = "desc",
            Products = new List<long> { 1 }
        };

        this.ProductRepository.Get(Arg.Any<long>()).Returns(null as Product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<CategoryServiceException>(() => sut.CreateCategory(categoryRequest));
    }

    #endregion

    #region Get

    [Fact]
    public async Task GetCategory_ValdidCategoryId_ReturnsCategory()
    {
        // Arrange
        var category = new Category("category", "desc", new List<ProductId>());
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
    public async Task GetCategories_ReturnsAllCategories()
    {
        // Arrange
        var categories = new[]
        {
            new Category("category 1", "desc", new List<ProductId>()),
            new Category("category 2", "desc", new List<ProductId>())
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
    public async Task UpdateCategory_InvalidCategoryId_ReturnsNull()
    {
        // Arrange
        var updateCategoryRequest = new RequestModels.Category();
        const int categoryId = 1;

        // Act
        var sut = this.GetServiceUnderTest();
        var category = await sut.UpdateCategory(categoryId, updateCategoryRequest);

        // Assert
        Assert.Null(category);
    }

    [Fact]
    public async Task UpdateCategory_ValidCategoryIdAndRequest_ReturnsUpdatedCategory()
    {
        // Arrange
        var oldCategory = new Category("old category", "old desc", new List<ProductId>());
        var categoryId = oldCategory.Id;

        var updateCategoryRequest = new RequestModels.Category
        {
            Name = "updated category",
            Description = "updated desc",
            Products = new List<long>()
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

    [Fact]
    public async Task UpdateCategory_InvalidCategoryNotAssociatedWithProduct_ThrowsException()
    {
        // Arrange
        var product = new List<ProductId> { new(1) };
        var category = new Category("category", "desc", product);

        this.CategoryRepository.Get(Arg.Any<long>()).Returns(category);
        this.ProductRepository.GetByCategoryId(category.Id).Returns(Array.Empty<Product>());

        var updateCategoryRequest = new RequestModels.Category
        {
            Name = "updated category",
            Description = "updated desc",
            Products = new List<long> { 1 }
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<CategoryServiceException>(() =>
            sut.UpdateCategory(category.Id, updateCategoryRequest));
    }

    [Fact]
    public async Task UpdateCategory_InvalidUpdatedProduct_ThrowsException()
    {
        // Arrange
        var oldProduct = CreateProduct(1, "product", "desc", 1.0);
        var oldProducts = new Product[] { oldProduct };
        var oldCategory = CreateCategory(1, "category", "desc", new List<ProductId> { new(1) });

        var updatedProduct = CreateProduct(2, "updated product", "desc", 1.0);

        this.CategoryRepository.Get(Arg.Any<long>()).Returns(oldCategory);
        this.ProductRepository.GetByCategoryId(oldCategory.Id).Returns(oldProducts);
        this.ProductRepository.Get(updatedProduct.Id).Returns(null as Product);

        var updatedCategory = new RequestModels.Category { Products = new List<long> { updatedProduct.Id } };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<CategoryServiceException>(() =>
            sut.UpdateCategory(oldCategory.Id, updatedCategory));
    }

    [Fact]
    public async Task UpdateCategory_AddProductRetired_ThrowsException()
    {
        // Arrange
        var oldProduct = CreateProduct(1, "product", "desc", 1.0);
        var oldProducts = new Product[]
        {
            oldProduct
        };
        var oldCategory = CreateCategory(1, "category", "desc", new List<ProductId> { new(1) });

        var updatedProduct = CreateProduct(2, "updated product", "desc", 10.0);
        updatedProduct.Retire();

        this.CategoryRepository.Get(Arg.Any<long>()).Returns(oldCategory);
        this.ProductRepository.GetByCategoryId(oldCategory.Id).Returns(oldProducts);
        this.ProductRepository.Get(updatedProduct.Id).Returns(updatedProduct);

        var updatedCategory = new RequestModels.Category
        {
            Name = "updated category",
            Description = "desc",
            Products = new List<long> { updatedProduct.Id }
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductLifecycleException>(() =>
            sut.UpdateCategory(oldCategory.Id, updatedCategory));
    }

    [Fact]
    public async Task UpdateCategory_RemoveProductRetired_ThrowsException()
    {
        // Arrange
        var oldProduct = CreateProduct(1, "product", "desc", 1.0);
        var oldProducts = new Product[]
        {
            oldProduct
        };
        var oldCategory = CreateCategory(1, "category", "desc", new List<ProductId> { new(1) });

        oldProduct.Retire();

        this.CategoryRepository.Get(Arg.Any<long>()).Returns(oldCategory);
        this.ProductRepository.GetByCategoryId(oldCategory.Id).Returns(oldProducts);

        var updatedCategory = new RequestModels.Category
        {
            Name = "updated category",
            Description = "desc",
            Products = new List<long>()
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductLifecycleException>(() =>
            sut.UpdateCategory(oldCategory.Id, updatedCategory));
    }

    [Fact]
    public async Task UpdateCategory_ValidUpdatedProduct_UpdatesProductCorrectly()
    {
        // Arrange
        var oldProduct = CreateProduct(1, "product", "desc", 1.0);
        var oldProducts = new Product[]
        {
            oldProduct
        };
        var oldCategory = CreateCategory(1, "category", "desc", new List<ProductId> { new(1) });

        var updatedProduct = CreateProduct(2, "updated product", "desc", 10.0);

        this.CategoryRepository.Get(Arg.Any<long>()).Returns(oldCategory);
        this.ProductRepository.GetByCategoryId(oldCategory.Id).Returns(oldProducts);
        this.ProductRepository.Get(updatedProduct.Id).Returns(updatedProduct);

        var updatedCategory = new RequestModels.Category
        {
            Name = "updated category",
            Description = "desc",
            Products = new List<long> { updatedProduct.Id }
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        var category = await sut.UpdateCategory(oldCategory.Id, updatedCategory);

        await this.ProductRepository.Received().GetByCategoryId(oldCategory.Id);
        Assert.Equal(updatedProduct.Id, category?.Products.First().Value);
    }

    #endregion

    #region Helpers

    public static Category CreateCategory(long id, string name, string description, IList<ProductId> products)
    {
        return categoryFactory.CreateCategory(id, name, description, DateTime.UtcNow, DateTime.UtcNow, products, false);
    }

    public static Product CreateProduct(long id, string name, string description, double price)
    {
        return productFactory.CreateProduct(id, name, description, price, new List<CategoryId>(), new List<TagId>(), false);
    }

    #endregion

    #region Setup

    private readonly ICategoryRepository CategoryRepository = Substitute.For<ICategoryRepository>();

    private readonly IProductRepository ProductRepository = Substitute.For<IProductRepository>();

    private ICategoryService GetServiceUnderTest()
    {
        return new CategoryService(this.CategoryRepository, this.ProductRepository);
    }

    #endregion
}
