using System;
using System.Linq;
using System.Reflection;
using Answer.King.Domain.Repositories.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories.Mappings;

public class ProductEntityMappings : IEntityMapping
{
    private static readonly ProductFactory ProductFactory = new();

    private static readonly FieldInfo? ProductIdFieldInfo =
        typeof(Product).GetField($"<{nameof(Product.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    public void RegisterMapping(BsonMapper mapper)
    {
        mapper.RegisterType(
            serialize: product =>
            {
                var tags = product.Tags.Select(c => new BsonValue(c.Value));

                var doc = new BsonDocument
                {
                    ["_id"] = product.Id,
                    ["name"] = product.Name,
                    ["description"] = product.Description,
                    ["price"] = product.Price,
                    ["createdOn"] = product.CreatedOn,
                    ["lastUpdated"] = product.LastUpdated,
                    ["Category"] = new BsonDocument
                    {
                        ["_id"] = product.Category.Id,
                        ["Name"] = product.Category.Name,
                        ["Description"] = product.Category.Description,
                    },
                    ["tags"] = new BsonArray(tags),
                    ["retired"] = product.Retired,
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;
                var cat = doc["Category"].AsDocument;
                var category = new ProductCategory(
                    cat["_id"].AsInt64,
                    cat["Name"].AsString,
                    cat["Description"].AsString);
                var tags = doc["tags"].AsArray.Select(c => new TagId(c)).ToList();

                return ProductFactory.CreateProduct(
                    doc["_id"].AsInt64,
                    doc["name"].AsString,
                    doc["description"].AsString,
                    doc["price"].AsDouble,
                    doc["createdOn"].AsDateTime,
                    doc["lastUpdated"].AsDateTime,
                    category,
                    tags,
                    doc["retired"].AsBoolean);
            });
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
