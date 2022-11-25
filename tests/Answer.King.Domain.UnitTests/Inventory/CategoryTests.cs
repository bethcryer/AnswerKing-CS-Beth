using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Domain.UnitTests.Inventory;

[TestCategory(TestType.Unit)]
public class CategoryTests
{
    [Fact]
    public void RenameCategory_WithValidNameAndDescription_ReturnsExpectedResult()
    {
        var category = new Category("Phones", "Electronics", new List<ProductId>());

        category.Rename("Lemon", "Squash");

        Assert.Equal("Lemon", category.Name);
        Assert.Equal("Squash", category.Description);
    }

    [Fact]
    public void RenameCategory_WithInvalidName_ThrowsException()
    {
        var category = new Category("Phones", "Electronics", new List<ProductId>());
        Assert.Throws<ArgumentNullException>(() => category.Rename(null!, "Electronics"));
    }

    [Fact]
    public void RenameCategory_WithBlankName_ThrowsException()
    {
        var category = new Category("Phones", "Electronics", new List<ProductId>());
        Assert.Throws<Guard.EmptyStringException>(() => category.Rename("", "Electronics"));
    }

    [Fact]
    public void RenameCategory_WithInvalidDescription_ThrowsException()
    {
        var category = new Category("Phones", "Electronics", new List<ProductId>());
        Assert.Throws<ArgumentNullException>(() => category.Rename("Phones", null!));
    }

    [Fact]
    public void RenameCategory_WithBlankDescription_ThrowsException()
    {
        var category = new Category("Phones", "Electronics", new List<ProductId>());
        Assert.Throws<Guard.EmptyStringException>(() => category.Rename("Phones", ""));
    }

    [Fact]
    public void RetireCategory_WithProductsContainedWithinCategory_ThrowsException()
    {
        var category = new Category("Phones", "Electronics", new List<ProductId>());
        category.AddProduct(new ProductId(1));

        Assert.Throws<CategoryLifecycleException>(() => category.RetireCategory());
    }
}
