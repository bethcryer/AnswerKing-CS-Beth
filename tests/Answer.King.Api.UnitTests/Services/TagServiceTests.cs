using Answer.King.Api.Services;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Infrastructure.Repositories.Mappings;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;
using Xunit;
using TagRequest = Answer.King.Api.RequestModels.Tag;

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
        var tagRequest = new TagRequest
        {
            Name = "Vegan",
            Description = "desc",
            Products = new List<long>(),
        };

        // Act
        var sut = this.GetServiceUnderTest();
        var actualTag = await sut.CreateTag(tagRequest);

        // Assert
        Assert.Equal(tagRequest.Name, actualTag.Name);
        Assert.Equal(tagRequest.Description, actualTag.Description);
    }

    [Fact]
    public async Task CreateTag_RetiredProduct_ThrowsException()
    {
        // Arrange
        var tagRequest = new TagRequest
        {
            Name = "Vegan",
            Description = "desc",
            Products = new List<long> { 1 },
        };

        this.productRepository.GetOne(1).Returns(null as Product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<TagServiceException>(() => sut.CreateTag(tagRequest));
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
        var updateTagRequest = new TagRequest();
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
        var oldTag = CreateTag(1, "old tag", "old desc", new List<ProductId> { new(1) });
        var product = CreateProduct(1, "product name", "product description", 1, new List<TagId> { new(1) });

        var updateTagRequest = new TagRequest
        {
            Name = "new tag",
            Description = "new desc",
            Products = new List<long> { 1 },
        };

        this.tagRepository.GetOne(oldTag.Id).Returns(oldTag);
        this.productRepository.GetOne(product.Id).Returns(product);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualTag = await sut.UpdateTag(oldTag.Id, updateTagRequest);

        // Assert
        Assert.Equal(updateTagRequest.Name, actualTag!.Name);
        Assert.Equal(updateTagRequest.Description, actualTag.Description);
        Assert.Equal(updateTagRequest.Products, actualTag.Products.Select(x => x.Value).ToList());

        await this.tagRepository.Received().GetOne(oldTag.Id);
        await this.tagRepository.Received().Save(Arg.Is<Tag>(x =>
            x.Name == updateTagRequest.Name && x.Description == updateTagRequest.Description));
    }

    [Fact]
    public async Task UpdateTag_ValidTagIdAndRequestWithAddedProducts_ReturnsUpdatedTag()
    {
        // Arrange
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId>());
        var product = CreateProduct(1, "product name", "product description", 1, new List<TagId>());

        var updateTagRequest = new TagRequest
        {
            Name = "tag",
            Description = "desc",
            Products = new List<long> { 1 },
        };

        this.tagRepository.GetOne(oldTag.Id).Returns(oldTag);
        this.productRepository.GetOne(product.Id).Returns(product);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualTag = await sut.UpdateTag(oldTag.Id, updateTagRequest);

        // Assert
        Assert.Equal(updateTagRequest.Name, actualTag!.Name);
        Assert.Equal(updateTagRequest.Description, actualTag.Description);
        Assert.Equal(updateTagRequest.Products, actualTag.Products.Select(x => x.Value).ToList());

        await this.tagRepository.Received().GetOne(oldTag.Id);
        await this.tagRepository.Received().Save(Arg.Is<Tag>(x => x.Products.Contains(new ProductId(product.Id))));
    }

    [Fact]
    public async Task UpdateTag_ValidTagIdAndRequestWithRemovedProducts_ReturnsUpdatedTag()
    {
        // Arrange
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId> { new(1) });
        var product = CreateProduct(1, "product name", "product description", 1, new List<TagId> { new(1) });

        var updateTagRequest = new TagRequest
        {
            Name = "tag",
            Description = "desc",
            Products = new List<long>(),
        };

        this.tagRepository.GetOne(oldTag.Id).Returns(oldTag);
        this.productRepository.GetOne(product.Id).Returns(product);

        // Act
        var sut = this.GetServiceUnderTest();
        var actualTag = await sut.UpdateTag(oldTag.Id, updateTagRequest);

        // Assert
        Assert.Equal(updateTagRequest.Name, actualTag!.Name);
        Assert.Equal(updateTagRequest.Description, actualTag.Description);
        Assert.Equal(updateTagRequest.Products, actualTag.Products.Select(x => x.Value).ToList());

        await this.tagRepository.Received().GetOne(oldTag.Id);
        await this.tagRepository.Received().Save(Arg.Is<Tag>(x => x.Products.Count == 0));
    }

    [Fact]
    public async Task UpdateTag_InvalidProduct_ThrowsException()
    {
        // Arrange
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId>());

        var updateTagRequest = new TagRequest
        {
            Name = "tag",
            Description = "desc",
            Products = new List<long> { 1 },
        };

        this.tagRepository.GetOne(oldTag.Id).Returns(oldTag);
        this.productRepository.GetOne(1).Returns(null as Product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<TagServiceException>(() => sut.UpdateTag(oldTag.Id, updateTagRequest));
    }

    [Fact]
    public async Task UpdateTag_RetiredTag_ThrowsException()
    {
        // Arrange
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId>(), true);
        var product = CreateProduct(1, "product name", "product description", 1, new List<TagId>());

        var updateTagRequest = new TagRequest
        {
            Name = "tag",
            Description = "desc",
            Products = new List<long> { 1 },
        };

        this.tagRepository.GetOne(oldTag.Id).Returns(oldTag);
        this.productRepository.GetOne(product.Id).Returns(product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<TagServiceException>(() => sut.UpdateTag(oldTag.Id, updateTagRequest));
    }

    [Fact]
    public async Task UpdateTag_AddRetiredProduct_ThrowsException()
    {
        // Arrange
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId>());
        var product = CreateProduct(1, "product name", "product description", 1, new List<TagId>(), true);

        var updateTagRequest = new TagRequest
        {
            Name = "tag",
            Description = "desc",
            Products = new List<long> { 1 },
        };

        this.tagRepository.GetOne(oldTag.Id).Returns(oldTag);
        this.productRepository.GetOne(product.Id).Returns(product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<TagServiceException>(() => sut.UpdateTag(oldTag.Id, updateTagRequest));
    }

    [Fact]
    public async Task UpdateTag_RemoveRetiredProduct_ThrowsException()
    {
        // Arrange
        var oldTag = CreateTag(1, "tag", "desc", new List<ProductId> { new(1) });
        var product = CreateProduct(1, "product name", "product description", 1, new List<TagId> { new(1) }, true);

        var updateTagRequest = new TagRequest
        {
            Name = "tag",
            Description = "desc",
            Products = new List<long>(),
        };

        this.tagRepository.GetOne(oldTag.Id).Returns(oldTag);
        this.productRepository.GetOne(product.Id).Returns(product);

        // Act / Assert
        var sut = this.GetServiceUnderTest();
        await Assert.ThrowsAsync<TagServiceException>(() => sut.UpdateTag(oldTag.Id, updateTagRequest));
    }

    #endregion

    #region Helpers

    private static Tag CreateTag(long id, string name, string description, IList<ProductId> products, bool retired = false)
    {
        return TagFactory.CreateTag(id, name, description, DateTime.UtcNow, DateTime.UtcNow, products, retired);
    }

    private static Product CreateProduct(long id, string name, string description, double price, IList<TagId> tags, bool retired = false)
    {
        return ProductFactory.CreateProduct(id, name, description, price, DateTime.UtcNow, DateTime.UtcNow, new ProductCategory(1, "category", "desc"), tags, retired);
    }

    #endregion

    #region Setup

    private ITagService GetServiceUnderTest()
    {
        return new TagService(this.tagRepository, this.productRepository);
    }

    #endregion
}
