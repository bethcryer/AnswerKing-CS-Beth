using System;
using System.Linq;
using System.Reflection;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories.Mappings;

public class CategoryEntityMappings : IEntityMapping
{
    private static readonly CategoryFactory categoryFactory = new();

    private static readonly FieldInfo? categoryIdFieldInfo =
        typeof(Category).GetField($"<{nameof(Category.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    public void RegisterMapping(BsonMapper mapper)
    {
        mapper.RegisterType
        (
            serialize: category =>
            {
                var productsIds = category.Products.Select(p => new BsonValue(p.Value));

                var doc = new BsonDocument
                {
                    ["_id"] = category.Id,
                    ["name"] = category.Name,
                    ["description"] = category.Description,
                    ["createdOn"] = category.CreatedOn,
                    ["lastUpdated"] = category.LastUpdated,
                    ["products"] = new BsonArray(productsIds.ToArray()),
                    ["retired"] = category.Retired
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;

                return categoryFactory.CreateCategory(
                    doc["_id"].AsInt64,
                    doc["name"].AsString,
                    doc["description"].AsString,
                    doc["createdOn"].AsDateTime,
                    doc["lastUpdated"].AsDateTime,
                    doc["products"].AsArray.Select(
                        p => new ProductId(p.AsInt64)).ToList(),
                    doc["retired"].AsBoolean);
            }
        );
    }

    public void ResolveMember(Type type, MemberInfo memberInfo, MemberMapper memberMapper)
    {
        if (type == typeof(Category) && memberMapper.MemberName == "Id")
        {
            memberMapper.Setter =
                (obj, value) => categoryIdFieldInfo?.SetValue(obj, value);
        }
    }
}
