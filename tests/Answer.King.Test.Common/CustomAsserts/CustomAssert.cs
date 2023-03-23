using System.Runtime.Serialization;

namespace Answer.King.Test.Common.CustomAsserts;

public static class CustomAssert
{
    public static void HasAttribute<T>(Type objectType)
        where T : Attribute
    {
        var attr = objectType.GetCustomAttributes(typeof(T), false).ToList();
        attr.AssertAttributeCount<T>();
    }

    public static void HasAttribute<T>(Enum @enum)
        where T : Attribute
    {
        var type = @enum.GetType();
        var memberInfo = type.GetMember(Enum.GetName(type, @enum) ?? throw new InvalidOperationException());
        var attr = memberInfo[0].GetCustomAttributes(typeof(T), false).ToList();

        attr.AssertAttributeCount<T>();
    }

    public static void MethodHasAttribute<T>(Type objectType, string methodName)
        where T : Attribute
    {
        var method = objectType.GetMethod(methodName) ??
                     throw new Exception($"Method {methodName} does not exist on type {objectType}.");

        var attr = method.GetCustomAttributes(typeof(T), false).ToList();
        attr.AssertAttributeCount<T>();
    }

    public static void PropertyHasAttribute<T>(Type objectType, string propertyName)
        where T : Attribute
    {
        var method = objectType.GetProperty(propertyName) ??
                     throw new Exception($"Property {propertyName} does not exist on type {objectType}.");

        var attr = method.GetCustomAttributes(typeof(T), false).ToList();
        attr.AssertAttributeCount<T>();
    }

    /// <summary>
    /// This is an empty function that simply serves to make tests more readable
    /// when checking that a certain action did not result in an exception.
    /// </summary>
    public static void DidNotThrow()
    {
        // Do nothing
    }

    public static void EnumMemberAttributeHasCorrectValue(Enum @enum, string expectedValue)
    {
        var type = @enum.GetType();
        var memberInfo = type.GetMember(Enum.GetName(type, @enum) ?? throw new InvalidOperationException());
        var attr = memberInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false).ToList();

        attr.AssertAttributeCountCorrectValue(expectedValue);
    }
}
