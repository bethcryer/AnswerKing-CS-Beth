using Answer.King.Api.RequestModels;
using Answer.King.Domain;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Api.UnitTests.RequestModels;

[TestCategory(TestType.Unit)]
public class CategoryTests
{
    [Fact]
    public void CategoryId_InitWithWithDefaultId_ThrowsDefaultValueException()
    {
        // Arrange
        const int id = 0;

        // Act / Assert
        Assert.Throws<Guard.DefaultValueException>(() => new CategoryId(id));
    }
}
