using System.Text;
using System.Text.Json;
using Answer.King.Api.Common.JsonConverters;
using Answer.King.Api.RequestModels;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Api.UnitTests.Common.CustomConverters;

[TestCategory(TestType.Unit)]
public class CategoryIdJsonConverterTests
{
    [Fact]
    public void Read_ValidInt64_ReturnsCategoryId()
    {
        // Arrange
        const string json = "1";
        var jsonUtf8Bytes = Encoding.UTF8.GetBytes(json);
        var jsonReader = new Utf8JsonReader(jsonUtf8Bytes);
        jsonReader.Read();

        var categoryIdJsonConverter = new CategoryIdJsonConverter();

        var expected = new CategoryId(1);

        // Act
        var result = categoryIdJsonConverter.Read(ref jsonReader, typeof(long), new JsonSerializerOptions());

        // Assert
        Assert.IsType<CategoryId>(result);
        Assert.Equal(result, expected);
    }

    [Fact]
    public void Read_InvalidInt64_ReturnsNull()
    {
        // Arrange
        const string json = "1.0";
        var jsonUtf8Bytes = Encoding.UTF8.GetBytes(json);
        var jsonReader = new Utf8JsonReader(jsonUtf8Bytes);
        jsonReader.Read();

        var categoryIdJsonConverter = new CategoryIdJsonConverter();

        // Act
        var result = categoryIdJsonConverter.Read(ref jsonReader, typeof(long), new JsonSerializerOptions());

        // Assert
        Assert.Null(result);
    }
}
