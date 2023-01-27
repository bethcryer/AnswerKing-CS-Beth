using Answer.King.Domain;
using Answer.King.Domain.Repositories.Models;
using Answer.King.Test.Common.CustomTraits;

namespace Answer.King.Infrastructure.UnitTests.Repositories.Models;

[TestCategory(TestType.Unit)]
public class TagTests
{
    [Fact]
    public void TagId_InitWithWithDefaultId_ThrowsDefaultValueException()
    {
        // Arrange
        const int id = 0;

        // Act / Assert
        Assert.Throws<Guard.DefaultValueException>(() => new TagId(id));
    }
}
