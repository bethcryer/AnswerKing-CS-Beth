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
public class TagServiceTests
{
    private static readonly ProductFactory ProductFactory = new();

    private static readonly TagFactory TagFactory = new();

    private readonly ITagRepository tagRepository = Substitute.For<ITagRepository>();

    private readonly IProductRepository productRepository = Substitute.For<IProductRepository>();

    #region Retire

    [Fact]
    public async Task RetireTag_InvalidTagIdReceived_ReturnsNull()
    {
        // Arrange
        this.tagRepository.GetOne(Arg.Any<long>()).Returns(null as Tag);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        Assert.Null(await sut.RetireTag(1));
    }

    [Fact]
    public async Task RetireTag_TagContainsProducts_ThrowsException()
    {
        // Arrange
        var tag = new Tag("tag", "desc", new List<ProductId>());
        tag.AddProduct(new ProductId(1));

        this.tagRepository.GetOne(tag.Id).Returns(tag);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<TagServiceException>(() =>
            sut.RetireTag(tag.Id));
    }

    [Fact]
    public async Task RetireTag_AlreadyRetired_ThrowsException()
    {
        // Arrange
        var tag = new Tag("tag", "desc", new List<ProductId>());
        tag.RetireTag();
        this.tagRepository.GetOne(tag.Id).Returns(tag);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<TagServiceException>(() =>
            sut.RetireTag(tag.Id));
    }

    [Fact]
    public async Task RetireTag_NoProductsAssociatedWithTag_ReturnsRetiredTag()
    {
        // Arrange
        var tag = new Tag("tag", "desc", new List<ProductId>());
        this.tagRepository.GetOne(tag.Id).Returns(tag);

        // Act
        var sut = this.GetServiceUnderTest();
        var retiredTag = await sut.RetireTag(tag.Id);

        // Assert
        Assert.True(retiredTag!.Retired);
    }

    #endregion

    #region Create

    [Fact]
    public async Task CreateTag_ValidTag_ReturnsNewTag()
    {
        // Arrange
        var tagRequest = new RequestModels.Tag
        {
            Name = "Vegan",
            Description = "desc",
        };

        // Act
        var sut = this.GetServiceUnderTest();
        var actualTag = await sut.CreateTag(tagRequest);

        // Assert
        Assert.Equal(tagRequest.Name, actualTag.Name);
        Assert.Equal(tagRequest.Description, actualTag.Description);
    }

    #endregion

    #region Get

    [Fact]
    public async Task GetCategory_ValidCategoryId_ReturnsCategory()
    {
        // Arrange
        var tag = new Tag("tag", "desc", new List<ProductId>());
        var id = tag.Id;

        this.tagRepository.GetOne(id).Returns(tag);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualTag = await sut.GetTag(id);

        // Assert
        Assert.Equal(tag, actualTag);
        await this.tagRepository.Received().GetOne(id);
    }

    [Fact]
    public async Task GetCategories_ReturnsAllCategories()
    {
        // Arrange
        var tags = new[]
        {
            new Tag("tag 1", "desc", new List<ProductId>()),
            new Tag("tag 2", "desc", new List<ProductId>()),
        };

        this.tagRepository.GetAll().Returns(tags);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualTags = await sut.GetTags();

        // Assert
        Assert.Equal(tags, actualTags);
        await this.tagRepository.Received().GetAll();
    }

    #endregion

    #region Update

    [Fact]
    public async Task UpdateTag_InvalidTagId_ReturnsNull()
    {
        // Arrange
        var updateTagRequest = new RequestModels.Tag();
        const int tagId = 1;

        // Act
        var sut = this.GetServiceUnderTest();
        var category = await sut.UpdateTag(tagId, updateTagRequest);

        // Assert
        Assert.Null(category);
    }

    [Fact]
    public async Task UpdateTag_ValidTagIdAndRequest_ReturnsUpdatedTag()
    {
        // Arrange
        var oldTag = new Tag("old tag", "old desc", new List<ProductId>());
        var tagId = oldTag.Id;

        var updateTagRequest = new RequestModels.Tag
        {
            Name = "updated category",
            Description = "updated desc",
        };

        this.tagRepository.GetOne(tagId).Returns(oldTag);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualTag = await sut.UpdateTag(tagId, updateTagRequest);

        // Assert
        Assert.Equal(updateTagRequest.Name, actualTag!.Name);
        Assert.Equal(updateTagRequest.Description, actualTag.Description);

        await this.tagRepository.Received().GetOne(tagId);
        await this.tagRepository.Received().Save(Arg.Any<Tag>());
    }

    #endregion

    #region Update: Add Products

    [Fact]
    public async Task AddTagProducts_InvalidTagId_ReturnsNull()
    {
        // Arrange
        var updateTagRequest = new RequestModels.TagProducts();
        const int tagId = 1;

        // Act
        var sut = this.GetServiceUnderTest();
        var category = await sut.AddProducts(tagId, updateTagRequest);

        // Assert
        Assert.Null(category);
    }

    [Fact]
    public async Task AddTagProducts_ValidTagIdAndRequest_ReturnsUpdatedTag()
    {
        // Arrange
        var oldTag = CreateTag(1, "old tag", "old desc", new List<ProductId>());
        var tagId = oldTag.Id;

        var product = CreateProduct(1, "Product", "desc", 1);
        var productId = product.Id;

        var addProducts = new RequestModels.TagProducts
        {
            Products = new List<long> { productId },
        };

        this.productRepository.GetOne(productId).Returns(product);
        this.tagRepository.GetOne(tagId).Returns(oldTag);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualTag = await sut.AddProducts(tagId, addProducts);

        // Assert
        Assert.Equal(addProducts.Products[0], actualTag!.Products.First().Value);

        await this.tagRepository.Received().GetOne(tagId);
        await this.tagRepository.Received().Save(Arg.Any<Tag>());
    }

    [Fact]
    public async Task AddTagProducts_InvalidTagNotAssociatedWithProduct_ThrowsException()
    {
        // Arrange
        var product = new List<ProductId> { new(1) };
        var tag = new Tag("tag", "desc", product);

        this.tagRepository.GetOne(Arg.Any<long>()).Returns(tag);
        this.productRepository.GetByCategoryId(tag.Id).Returns(Array.Empty<Product>());

        var addProducts = new RequestModels.TagProducts
        {
            Products = new List<long> { 1 },
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<TagServiceException>(() =>
            sut.AddProducts(tag.Id, addProducts));
    }

    [Fact]
    public async Task AddTagProducts_InvalidUpdatedProduct_ThrowsException()
    {
        // Arrange
        var oldProduct = CreateProduct(1, "product", "desc", 1.0);
        var oldProducts = new[] { oldProduct };
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId> { new(1) });

        var updatedProduct = CreateProduct(2, "updated product", "desc", 1.0);

        this.tagRepository.GetOne(Arg.Any<long>()).Returns(oldTag);
        this.productRepository.GetByCategoryId(oldTag.Id).Returns(oldProducts);
        this.productRepository.GetOne(updatedProduct.Id).Returns(null as Product);

        var addProducts = new RequestModels.TagProducts
        {
            Products = new List<long> { updatedProduct.Id },
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<TagServiceException>(() =>
            sut.AddProducts(oldTag.Id, addProducts));
    }

    [Fact]
    public async Task AddTagProducts_TagRetired_ThrowsException()
    {
        // Arrange
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId>());

        var updatedProduct = CreateProduct(2, "updated product", "desc", 1.0);
        updatedProduct.Retire();

        this.tagRepository.GetOne(Arg.Any<long>()).Returns(oldTag);
        this.productRepository.GetOne(updatedProduct.Id).Returns(updatedProduct);

        var addProducts = new RequestModels.TagProducts
        {
            Products = new List<long> { updatedProduct.Id },
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductLifecycleException>(() =>
            sut.AddProducts(oldTag.Id, addProducts));
    }

    [Fact]
    public async Task AddTagProducts_ValidUpdatedProduct_UpdatesProductCorrectly()
    {
        // Arrange
        var oldProduct = CreateProduct(1, "product", "desc", 1.0);
        var oldProducts = new[]
        {
            oldProduct,
        };
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId> { new(1) });

        var updatedProduct = CreateProduct(2, "updated product", "desc", 10.0);

        this.tagRepository.GetOne(Arg.Any<long>()).Returns(oldTag);
        this.productRepository.GetByCategoryId(oldTag.Id).Returns(oldProducts);
        this.productRepository.GetOne(updatedProduct.Id).Returns(updatedProduct);

        var addProducts = new RequestModels.TagProducts
        {
            Products = new List<long> { updatedProduct.Id },
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        var tag = await sut.AddProducts(oldTag.Id, addProducts);
        Assert.Equal(oldProduct.Id, tag?.Products.First().Value);
        Assert.Equal(updatedProduct.Id, tag?.Products.ElementAt(1).Value);
    }

    #endregion

    #region Delete: Remove Products

    [Fact]
    public async Task RemoveTagProducts_InvalidTagId_ReturnsNull()
    {
        // Arrange
        var updateTagRequest = new RequestModels.TagProducts();
        const int tagId = 1;

        // Act
        var sut = this.GetServiceUnderTest();
        var category = await sut.RemoveProducts(tagId, updateTagRequest);

        // Assert
        Assert.Null(category);
    }

    [Fact]
    public async Task RemoveTagProducts_ValidTagIdAndRequest_ReturnsUpdatedTag()
    {
        // Arrange
        var product = CreateProduct(1, "Product", "desc", 1);
        var productId = product.Id;

        var oldTag = CreateTag(1, "old tag", "old desc", new List<ProductId> { new(productId) });
        var tagId = oldTag.Id;

        var removeProducts = new RequestModels.TagProducts
        {
            Products = new List<long> { productId },
        };

        this.productRepository.GetOne(productId).Returns(product);
        this.tagRepository.GetOne(tagId).Returns(oldTag);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualTag = await sut.RemoveProducts(tagId, removeProducts);

        // Assert
        Assert.Empty(actualTag!.Products);

        await this.tagRepository.Received().GetOne(tagId);
        await this.tagRepository.Received().Save(Arg.Any<Tag>());
    }

    [Fact]
    public async Task RemoveTagProducts_InvalidTagNotAssociatedWithProduct_ThrowsException()
    {
        // Arrange
        var product = new List<ProductId> { new(1) };
        var tag = new Tag("tag", "desc", product);

        this.tagRepository.GetOne(Arg.Any<long>()).Returns(tag);
        this.productRepository.GetByCategoryId(tag.Id).Returns(Array.Empty<Product>());

        var removeProducts = new RequestModels.TagProducts
        {
            Products = new List<long> { 1 },
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<TagServiceException>(() =>
            sut.RemoveProducts(tag.Id, removeProducts));
    }

    [Fact]
    public async Task RemoveTagProducts_InvalidUpdatedProduct_ThrowsException()
    {
        // Arrange
        var oldProduct = CreateProduct(1, "product", "desc", 1.0);
        var oldProducts = new[] { oldProduct };
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId> { new(1) });

        var updatedProduct = CreateProduct(2, "updated product", "desc", 1.0);

        this.tagRepository.GetOne(Arg.Any<long>()).Returns(oldTag);
        this.productRepository.GetByCategoryId(oldTag.Id).Returns(oldProducts);
        this.productRepository.GetOne(updatedProduct.Id).Returns(null as Product);

        var removeProducts = new RequestModels.TagProducts
        {
            Products = new List<long> { updatedProduct.Id },
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<TagServiceException>(() =>
            sut.RemoveProducts(oldTag.Id, removeProducts));
    }

    [Fact]
    public async Task RemoveTagProducts_ProductRetired_ThrowsException()
    {
        // Arrange
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId>());

        var updatedProduct = CreateProduct(2, "updated product", "desc", 1.0);
        updatedProduct.Retire();

        this.tagRepository.GetOne(Arg.Any<long>()).Returns(oldTag);
        this.productRepository.GetOne(updatedProduct.Id).Returns(updatedProduct);

        var removeProducts = new RequestModels.TagProducts
        {
            Products = new List<long> { updatedProduct.Id },
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<ProductLifecycleException>(() =>
            sut.RemoveProducts(oldTag.Id, removeProducts));
    }

    [Fact]
    public async Task RemoveTagProducts_ValidUpdatedProduct_UpdatesProductCorrectly()
    {
        // Arrange
        var oldProduct = CreateProduct(1, "product", "desc", 1.0);
        var oldProducts = new[]
        {
            oldProduct,
        };
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId> { new(1) });

        this.tagRepository.GetOne(Arg.Any<long>()).Returns(oldTag);
        this.productRepository.GetByCategoryId(oldTag.Id).Returns(oldProducts);
        this.productRepository.GetOne(oldProduct.Id).Returns(oldProduct);

        var removeProducts = new RequestModels.TagProducts
        {
            Products = new List<long> { oldProduct.Id },
        };

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        var tag = await sut.RemoveProducts(oldTag.Id, removeProducts);
        Assert.Empty(tag!.Products);
    }

    #endregion

    #region Helpers

    private static Tag CreateTag(long id, string name, string description, IList<ProductId> products)
    {
        return TagFactory.CreateTag(id, name, description, DateTime.UtcNow, DateTime.UtcNow, products, false);
    }

    private static Product CreateProduct(long id, string name, string description, double price)
    {
        return ProductFactory.CreateProduct(id, name, description, price, new List<CategoryId>(), new List<TagId>(), false);
    }

    #endregion

    #region Setup

    private ITagService GetServiceUnderTest()
    {
        return new TagService(this.tagRepository, this.productRepository);
    }

    #endregion
}
