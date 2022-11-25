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

namespace Answer.King.Api.UnitTests.Services;

[TestCategory(TestType.Unit)]
public class ProductServiceTests
{
    #region Create

    [Fact]
    public async void CreateProduct_ValidProduct_ReturnsNewlyCreatedProduct()
    {
        // Arrange
        var request = new RequestModels.Product
        {
            Name = "product",
            Description = "desc",
        };

        // Act
        var sut = this.GetServiceUnderTest();
        var category = await sut.CreateProduct(request);

        // Assert
        Assert.Equal(request.Name, category.Name);
        Assert.Equal(request.Description, category.Description);

        await this.ProductRepository.Received().AddOrUpdate(Arg.Any<Product>());
    }

    #endregion

    #region Retire

    [Fact]
    public async void RetireProduct_InvalidProductId_ReturnsNull()
    {
        // Arrange
        this.ProductRepository.Get(Arg.Any<long>()).Returns(null as Domain.Repositories.Models.Product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        Assert.Null(await sut.RetireProduct(1));
    }

    [Fact]
    public async void RetireProduct_ValidProductId_ReturnsProductAsRetired()
    {
        // Arrange
        var product = ProductFactory.CreateProduct(1,
            "product", "desc", 12.00, new List<CategoryId> { new(1) }, false);

        this.ProductRepository.Get(product.Id).Returns(product);

        var category = new Category("category", "desc", new List<ProductId>());
        var categories = new Category[] { category };

        this.CategoryRepository.GetByProductId(product.Id)
            .Returns(categories);

        // Act
        var sut = this.GetServiceUnderTest();
        var retiredProduct = await sut.RetireProduct(product.Id);

        // Assert
        Assert.True(retiredProduct!.Retired);
        Assert.Equal(product.Id, retiredProduct.Id);

        await this.CategoryRepository.Received().GetByProductId(product.Id);
        await this.CategoryRepository.Save(category);
        await this.ProductRepository.AddOrUpdate(product);
    }

    #endregion

    #region Update

    [Fact]
    public async void UpdateProduct_InvalidProductId_ReturnsNull()
    {
        // Arrange
        this.ProductRepository.Get(Arg.Any<long>()).Returns(null as Product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        Assert.Null(await sut.UpdateProduct(1, new RequestModels.Product()));
    }

    #endregion

    #region Get

    [Fact]
    public async void GetProducts_ReturnsAllProducts()
    {
        // Arrange
        var products = new[]
        {
            new Product("product 1", "desc", 10.00),
            new Product("product 2", "desc", 5.00)
        };

        this.ProductRepository.Get().Returns(products);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualProducts = await sut.GetProducts();

        // Assert
        Assert.Equal(products, actualProducts);
        await this.ProductRepository.Received().Get();
    }

    [Fact]
    public async void GetProduct_ValidProductId_ReturnsProduct()
    {
        // Arrange
        var categories = new List<Domain.Repositories.Models.CategoryId> { new(1) };
        var product = new Domain.Repositories.Models.Product("product 1", "desc", 10.00);

        this.ProductRepository.Get(product.Id).Returns(product);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualProduct = await sut.GetProduct(product.Id);

        // Assert
        Assert.Equal(product, actualProduct);
        await this.ProductRepository.Received().Get(product.Id);
    }

    #endregion

    #region Helpers

    public Category CreateCategory(long id, string name, string description)
    {
        return CategoryFactory.CreateCategory(id, name, description, DateTime.UtcNow, DateTime.UtcNow, new List<Answer.King.Domain.Inventory.Models.ProductId>(), false);
    }

    #endregion

    #region Setup

    private readonly ICategoryRepository CategoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IProductRepository ProductRepository = Substitute.For<IProductRepository>();

    private IProductService GetServiceUnderTest()
    {
        return new ProductService(this.ProductRepository, this.CategoryRepository);
    }

    #endregion
}
