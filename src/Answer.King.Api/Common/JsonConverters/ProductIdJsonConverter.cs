using System.Text.Json;
using System.Text.Json.Serialization;
using Answer.King.Domain.Inventory.Models;

namespace Answer.King.Api.Common.JsonConverters;

public class ProductIdJsonConverter : JsonConverter<ProductId>
{
    public override ProductId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetInt64(out var id))
        {
            return new ProductId(id);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, ProductId value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
