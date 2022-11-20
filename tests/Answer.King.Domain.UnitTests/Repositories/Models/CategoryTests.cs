using Answer.King.Domain.Repositories.Models;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Domain.UnitTests.Repositories.Models;

[TestCategory(TestType.Unit)]
public class CategoryTests
{
    [Fact]
    public void CategoryId_InitWithWithDefaultId_ThrowsDefaultValueException()
    {
        // Arrange
        var id = 0;

        // Act / Assert
        Assert.Throws<Guard.DefaultValueException>(() => new CategoryId(id));
    }
}
