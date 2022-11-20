using System;
using System.Linq;
using System.Reflection;
using Answer.King.Domain.Orders;
using Answer.King.Domain.Orders.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories.Mappings;

public class OrderEntityMappings : IEntityMapping
{
    private static readonly FieldInfo? OrderIdFieldInfo =
        typeof(Order).GetField($"<{nameof(Order.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    public void RegisterMapping(BsonMapper mapper)
    {
        mapper.RegisterType
        (
            serialize: order =>
            {
                var lineItems = order.LineItems.Select(li => new BsonDocument
                {
                    ["product"] = new BsonDocument
                    {
                        ["_id"] = li.Product.Id,
                        ["name"] = li.Product.Name,
                        ["description"] = li.Product.Description,
                        ["categories"] = new BsonArray(li.Product.Categories.Select(c => new BsonDocument
                        {
                            ["_id"] = c.Id,
                            ["name"] = c.Name,
                            ["description"] = c.Description
                        })),
                        ["price"] = li.Product.Price
                    },
                    ["quantity"] = li.Quantity
                });

                var doc = new BsonDocument
                {
                    ["_id"] = order.Id,
                    ["createdOn"] = order.CreatedOn,
                    ["lastUpdated"] = order.LastUpdated,
                    ["orderStatus"] = $"{order.OrderStatus}",
                    ["lineItems"] = new BsonArray(lineItems)
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;

                var lineItems =
                    doc["lineItems"].AsArray.Select(this.ToLineItem)
                        .ToList();

                return OrderFactory.CreateOrder(
                    doc["_id"].AsInt64,
                    doc["createdOn"].AsDateTime,
                    doc["lastUpdated"].AsDateTime,
                    (OrderStatus)Enum.Parse(typeof(OrderStatus), doc["orderStatus"]),
                    lineItems);
            }
        );
    }

    public void ResolveMember(Type type, MemberInfo memberInfo, MemberMapper memberMapper)
    {
        if (type == typeof(Order) && memberMapper.MemberName == "Id")
        {
            memberMapper.Setter =
                (obj, value) => OrderIdFieldInfo?.SetValue(obj, value);
        }
    }

    private LineItem ToLineItem(BsonValue item)
    {
        var lineItem = item.AsDocument;
        var product = lineItem["product"].AsDocument;
        var category = product["category"].AsDocument;

        var result = new LineItem(
            new Product(
                product["_id"].AsInt64,
                product["name"].AsString,
                product["description"].AsString,
                product["price"].AsDouble,
                product["Categories"].AsArray.Select(p =>
                    new Category(
                        p.AsDocument["_id"].AsInt64,
                        p.AsDocument["name"].AsString,
                        p.AsDocument["description"].AsString
                     )
                ).ToList()
            )
        );

        result.AddQuantity(lineItem["quantity"].AsInt32);

        return result;
    }
}
