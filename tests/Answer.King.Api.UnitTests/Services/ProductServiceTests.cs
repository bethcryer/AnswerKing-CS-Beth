using Answer.King.Api.Services;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;
using Xunit;
using Category = Answer.King.Domain.Inventory.Category;
using Product = Answer.King.Domain.Repositories.Models.Product;
using ProductRequest = Answer.King.Api.RequestModels.Product;

namespace Answer.King.Api.UnitTests.Services;

[TestCategory(TestType.Unit)]
public class ProductServiceTests
{
    private static readonly ProductFactory ProductFactory = new();

    private readonly ICategoryRepository categoryRepository = Substitute.For<ICategoryRepository>();

    private readonly IProductRepository productRepository = Substitute.For<IProductRepository>();

    #region Create

    [Fact]
    public async Task CreateProduct_InValidCategoryId_ThrowsException()
    {
        // Arrange
        var request = new ProductRequest
        {
            Name = "product",
            Description = "desc",
            Price = 1500.00,
            CategoryId = new Api.RequestModels.CategoryId(2),
        };
        this.categoryRepository.GetOne(Arg.Any<long>()).Returns(null as Category);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductServiceException>(() => sut.CreateProduct(request));
    }

    #endregion

    #region Retire

    [Fact]
    public async Task RetireProduct_InvalidProductId_ReturnsNull()
    {
        // Arrange
        this.productRepository.GetOne(Arg.Any<long>()).Returns(null as Product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        Assert.Null(await sut.RetireProduct(1));
    }

    [Fact]
    public async Task RetireProduct_ValidProductId_ReturnsProductAsRetired()
    {
        // Arrange
        var product = ProductFactory.CreateProduct(
            1, "product", "desc", 12.00, new ProductCategory(1, "category", "desc"), new List<TagId> { new(1) }, false);

        this.productRepository.GetOne(product.Id).Returns(product);

        var category = new Category("category", "desc", new List<ProductId>());
        var categories = new[] { category };

        this.categoryRepository.GetByProductId(product.Id)
            .Returns(categories);

        // Act
        var sut = this.GetServiceUnderTest();
        var retiredProduct = await sut.RetireProduct(product.Id);

        // Assert
        Assert.True(retiredProduct!.Retired);
        Assert.Equal(product.Id, retiredProduct.Id);

        await this.categoryRepository.Received().GetByProductId(product.Id);
        await this.categoryRepository.Save(category);
        await this.productRepository.AddOrUpdate(product);
    }

    #endregion

    #region Update

    [Fact]
    public async Task UpdateProduct_InvalidProductId_ReturnsNull()
    {
        // Arrange
        this.productRepository.GetOne(Arg.Any<long>()).Returns(null as Product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        Assert.Null(await sut.UpdateProduct(1, new ProductRequest()));
    }

    #endregion

    #region Get

    [Fact]
    public async Task GetProducts_ReturnsAllProducts()
    {
        // Arrange
        var products = new[]
        {
            new Product("product 1", "desc", 10.00, new ProductCategory(1, "category", "desc")),
            new Product("product 2", "desc", 5.00, new ProductCategory(2, "category", "desc")),
        };

        this.productRepository.GetAll().Returns(products);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualProducts = await sut.GetProducts();

        // Assert
        Assert.Equal(products, actualProducts);
        await this.productRepository.Received().GetAll();
    }

    [Fact]
    public async Task GetProduct_ValidProductId_ReturnsProduct()
    {
        // Arrange
        var product = new Product("product 1", "desc", 10.00, new ProductCategory(2, "category", "desc"));

        this.productRepository.GetOne(product.Id).Returns(product);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualProduct = await sut.GetProduct(product.Id);

        // Assert
        Assert.Equal(product, actualProduct);
        await this.productRepository.Received().GetOne(product.Id);
    }

    #endregion

    #region Setup

    private IProductService GetServiceUnderTest()
    {
        return new ProductService(this.productRepository, this.categoryRepository);
    }

    #endregion
}
