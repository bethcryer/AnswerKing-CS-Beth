using System;
using System.Linq;
using System.Reflection;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories.Mappings;

public class TagsEntityMappings : IEntityMapping
{
    private static readonly TagFactory tagFactory = new();

    private static readonly FieldInfo? tagIdFieldInfo =
        typeof(Tag).GetField($"<{nameof(Tag.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    public void RegisterMapping(BsonMapper mapper)
    {
        mapper.RegisterType
        (
            serialize: tag =>
            {
                var productsIds = tag.Products.Select(p => new BsonValue(p.Value));

                var doc = new BsonDocument
                {
                    ["_id"] = tag.Id,
                    ["name"] = tag.Name,
                    ["description"] = tag.Description,
                    ["createdOn"] = tag.CreatedOn,
                    ["lastUpdated"] = tag.LastUpdated,
                    ["products"] = new BsonArray(productsIds.ToArray()),
                    ["retired"] = tag.Retired
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;

                return tagFactory.CreateTag(
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
        if (type == typeof(Tag) && memberMapper.MemberName == "Id")
        {
            memberMapper.Setter =
                (obj, value) => tagIdFieldInfo?.SetValue(obj, value);
        }
    }
}
