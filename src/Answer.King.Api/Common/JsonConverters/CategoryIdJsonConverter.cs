using System.Text.Json;
using System.Text.Json.Serialization;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Api.Common.JsonConverters;

public class CategoryIdJsonConverter : JsonConverter<CategoryId>
{
    public override CategoryId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetInt64(out long id))
        {
            return new CategoryId(id);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, CategoryId value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
