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
                    ["Product"] = new BsonDocument
                    {
                        ["_id"] = li.Product.Id,
                        ["Name"] = li.Product.Name,
                        ["Description"] = li.Product.Description,
                        ["Categories"] = new BsonArray(li.Product.Categories.Select(c => new BsonDocument
                        {
                            ["_id"] = c.Id,
                            ["Name"] = c.Name,
                            ["Description"] = c.Description
                        })),
                        ["Price"] = li.Product.Price
                    },
                    ["Quantity"] = li.Quantity
                });

                var doc = new BsonDocument
                {
                    ["_id"] = order.Id,
                    ["CreatedOn"] = order.CreatedOn,
                    ["LastUpdated"] = order.LastUpdated,
                    ["OrderStatus"] = $"{order.OrderStatus}",
                    ["LineItems"] = new BsonArray(lineItems)
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;

                var lineItems =
                    doc["LineItems"].AsArray.Select(this.ToLineItem)
                        .ToList();

                return OrderFactory.CreateOrder(
                    doc["_id"].AsInt64,
                    doc["CreatedOn"].AsDateTime,
                    doc["LastUpdated"].AsDateTime,
                    (OrderStatus)Enum.Parse(typeof(OrderStatus), doc["OrderStatus"]),
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
        var product = lineItem["Product"].AsDocument;
        var category = product["Category"].AsDocument;

        var result = new LineItem(
            new Product(
                product["_id"].AsInt64,
                product["Name"].AsString,
                product["Description"].AsString,
                product["Price"].AsDouble,
                product["Categories"].AsArray.Select(p =>
                    new Category(
                        p.AsDocument["_id"].AsInt64,
                        p.AsDocument["Name"].AsString,
                        p.AsDocument["Description"].AsString
                     )
                ).ToList()
            )
        );

        result.AddQuantity(lineItem["Quantity"].AsInt32);

        return result;
    }
}
