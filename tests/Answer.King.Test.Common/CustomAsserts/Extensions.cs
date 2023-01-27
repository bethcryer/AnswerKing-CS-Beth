using System.Runtime.Serialization;

namespace Answer.King.Test.Common.CustomAsserts;

internal static class Extensions
{
    internal static void AssertAttributeCount<T>(this ICollection<object> attr)
        where T : Attribute
    {
        var baseMessage = $"Assert has {typeof(T).Name} attribute failure.";

        if (attr.Count == 0)
        {
            throw new Exception($"{baseMessage} No such attribute.");
        }

        if (attr.Count > 1)
        {
            throw new Exception(
                $"{baseMessage} Attribute exists {attr.Count} times.");
        }
    }

    internal static void AssertAttributeCountCorrectValue(this ICollection<object> attr, string value)
    {
        var baseMessage = $"Assert has {nameof(EnumMemberAttribute)} attribute failure.";

        if (attr.Count == 0)
        {
            throw new Exception($"{baseMessage} No such attribute.");
        }

        if (attr.Count > 1)
        {
            throw new Exception(
                $"{baseMessage} Attribute exists {attr.Count} times.");
        }

        var attribute = (EnumMemberAttribute)attr.First();

        if (!string.Equals(attribute.Value, value, StringComparison.Ordinal))
        {
            throw new Exception(
                $"{baseMessage} Attribute value expected to be '{value}' but got '{attribute.Value}'.");
        }
    }
}
