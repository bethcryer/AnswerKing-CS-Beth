using System;
using System.Reflection;
using Answer.King.Domain.Repositories.Models;
using LiteDB;

namespace Answer.King.Infrastructure.Repositories.Mappings;

public class PaymentEntityMappings : IEntityMapping
{
    private static readonly FieldInfo? PaymentIdFieldInfo =
        typeof(Payment).GetField($"<{nameof(Payment.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    public void RegisterMapping(BsonMapper mapper)
    {
        mapper.RegisterType
        (
            serialize: payment =>
            {
                var doc = new BsonDocument
                {
                    ["_id"] = payment.Id,
                    ["orderId"] = payment.OrderId,
                    ["amount"] = payment.Amount,
                    ["orderTotal"] = payment.OrderTotal,
                    ["date"] = payment.Date
                };

                return doc;
            },
            deserialize: bson =>
            {
                var doc = bson.AsDocument;

                return PaymentFactory.CreatePayment(
                    doc["_id"].AsInt64,
                    doc["orderId"].AsInt64,
                    doc["amount"].AsDouble,
                    doc["orderTotal"].AsDouble,
                    doc["date"].AsDateTime);
            }
        );
    }

    public void ResolveMember(Type type, MemberInfo memberInfo, MemberMapper memberMapper)
    {
        if (type == typeof(Payment) && memberMapper.MemberName == "Id")
        {
            memberMapper.Setter =
                (obj, value) => PaymentIdFieldInfo?.SetValue(obj, value);
        }
    }
}
