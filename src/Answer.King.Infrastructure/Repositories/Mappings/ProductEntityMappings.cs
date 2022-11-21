using System.Linq;
using System;
using System.Reflection;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Repositories.Models;
using LiteDB;
using System.Collections.Generic;
using Category = Answer.King.Domain.Repositories.Models.Category;

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
                var categories = product.Categories.Select(p => new BsonDocument
                {
                    ["_id"] = p.Id,
                    ["Name"] = p.Name,
                    ["Description"] = p.Description
                });

                var doc = new BsonDocument
                {
                    ["_id"] = product.Id,
                    ["Name"] = product.Name,
                    ["Description"] = product.Description,
                    ["Price"] = product.Price,
                    ["Categories"] = new BsonArray(categories),
                    ["Retired"] = product.Retired
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;

                return ProductFactory.CreateProduct(
                    doc["_id"].AsInt64,
                    doc["Name"].AsString,
                    doc["Description"].AsString,
                    doc["Price"].AsDouble,
                    doc["Categories"].AsArray.Select(
                        p => new Category(
                            p.AsDocument["_id"].AsInt64,
                            p.AsDocument["Name"].AsString,
                            p.AsDocument["Description"].AsString
                            )).ToList(),
                    doc["Retired"].AsBoolean);
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
