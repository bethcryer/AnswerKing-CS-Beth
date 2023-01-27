using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Domain.UnitTests.Inventory;

[TestCategory(TestType.Unit)]
public class TagTests
{
    [Fact]
    public void RenameTag_WithValidNameAndDescription_ReturnsExpectedResult()
    {
        var tag = new Tag("Vegan", "Non-animal products", new List<ProductId>());

        tag.Rename("BOGOF", "Buy one get one free!");

        Assert.Equal("BOGOF", tag.Name);
        Assert.Equal("Buy one get one free!", tag.Description);
    }

    [Fact]
    public void RenameTag_WithInvalidName_ThrowsException()
    {
        var tag = new Tag("Vegan", "Non-animal products", new List<ProductId>());
        Assert.Throws<ArgumentNullException>(() => tag.Rename(null!, "Buy one get one free!"));
    }

    [Fact]
    public void RenameTag_WithBlankName_ThrowsException()
    {
        var tag = new Tag("Vegan", "Non-animal products", new List<ProductId>());
        Assert.Throws<Guard.EmptyStringException>(() => tag.Rename(string.Empty, "Buy one get one free!"));
    }

    [Fact]
    public void RenameTag_WithInvalidDescription_ThrowsException()
    {
        var tag = new Tag("Vegan", "Non-animal products", new List<ProductId>());
        Assert.Throws<ArgumentNullException>(() => tag.Rename("BOGOF", null!));
    }

    [Fact]
    public void RenameTag_WithBlankDescription_ThrowsException()
    {
        var tag = new Tag("Vegan", "Non-animal products", new List<ProductId>());
        Assert.Throws<Guard.EmptyStringException>(() => tag.Rename("BOGOF", string.Empty));
    }

    [Fact]
    public void RetireTag_WithProductsContainedWithinTag_ThrowsException()
    {
        var tag = new Tag("Vegan", "Non-animal products", new List<ProductId>());
        tag.AddProduct(new ProductId(1));

        Assert.Throws<TagLifecycleException>(tag.RetireTag);
    }

    [Fact]
    public void AddProduct_WithValidIdAndNotRetired_ReturnsExpectedResult()
    {
        var tag = new Tag("Vegan", "Non-animal products", new List<ProductId>());
        var productId = new ProductId(1);

        tag.AddProduct(productId);

        Assert.Equal(tag.Products.First(), productId);
    }

    [Fact]
    public void AddProduct_TagRetired_ThrowsException()
    {
        var tag = new Tag("Vegan", "Non-animal products", new List<ProductId>());
        tag.RetireTag();

        Assert.Throws<TagLifecycleException>(() => tag.AddProduct(new ProductId(1)));
    }

    [Fact]
    public void RemoveProduct_WithValidIdAndNotRetired_ReturnsExpectedResult()
    {
        var tag = new Tag("Vegan", "Non-animal products", new List<ProductId> { new(1) });
        tag.RemoveProduct(new ProductId(1));

        Assert.Empty(tag.Products);
    }

    [Fact]
    public void RemoveProduct_TagRetired_ThrowsException()
    {
        var tag = new Tag("Vegan", "Non-animal products", new List<ProductId>());
        tag.RetireTag();

        Assert.Throws<TagLifecycleException>(() => tag.RemoveProduct(new ProductId(1)));
    }
}
