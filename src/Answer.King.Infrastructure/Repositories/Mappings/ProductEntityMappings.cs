using System.Linq;
using System;
using System.Reflection;
using Answer.King.Domain.Repositories.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories.Mappings;

public class ProductEntityMappings : IEntityMapping
{
    private static readonly FieldInfo? ProductIdFieldInfo =
        typeof(Product).GetField($"<{nameof(Product.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    public void RegisterMapping(BsonMapper mapper)
    {
        mapper.RegisterType
        (
            serialize: product =>
            {
                var categories = product.Categories.Select(c => new BsonValue(c.Value));

                var doc = new BsonDocument
                {
                    ["_id"] = product.Id,
                    ["name"] = product.Name,
                    ["description"] = product.Description,
                    ["price"] = product.Price,
                    ["categories"] = new BsonArray(categories),
                    ["retired"] = product.Retired
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;
                var categories = doc["categories"].AsArray.Select(c => new CategoryId(c)).ToList();

                return ProductFactory.CreateProduct(
                    doc["_id"].AsInt64,
                    doc["name"].AsString,
                    doc["description"].AsString,
                    doc["price"].AsDouble,
                    categories,
                    doc["retired"].AsBoolean);
            }
        );
    }

    public void ResolveMember(Type type, MemberInfo memberInfo, MemberMapper memberMapper)
    {
        if (type == typeof(Product) && memberMapper.MemberName == "Id")
        {
            memberMapper.Setter =
                (obj, value) => ProductIdFieldInfo?.SetValue(obj, value);
        }
    }
}
