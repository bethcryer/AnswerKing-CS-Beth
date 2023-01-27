using System.Text;
using System.Text.Json;
using Answer.King.Api.Common.JsonConverters;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Test.Common.CustomTraits;
using Xunit;

namespace Answer.King.Api.UnitTests.Common.CustomConverters;

[TestCategory(TestType.Unit)]
public class ProductIdJsonConverterTests
{
    [Fact]
    public void Read_ValidInt64_ReturnsProductId()
    {
        // Arrange
        const string json = "1";
        var jsonUtf8Bytes = Encoding.UTF8.GetBytes(json);
        var jsonReader = new Utf8JsonReader(jsonUtf8Bytes);
        jsonReader.Read();

        var productIdJsonConverter = new ProductIdJsonConverter();

        var expected = new ProductId(1);

        // Act
        var result = productIdJsonConverter.Read(ref jsonReader, typeof(long), new JsonSerializerOptions());

        // Assert
        Assert.IsType<ProductId>(result);
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

        var productIdJsonConverter = new ProductIdJsonConverter();

        // Act
        var result = productIdJsonConverter.Read(ref jsonReader, typeof(long), new JsonSerializerOptions());

        // Assert
        Assert.Null(result);
    }
}
