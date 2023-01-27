using Xunit.Abstractions;
using Xunit.Sdk;

namespace Answer.King.Test.Common.CustomTraits;

/// <summary>
/// Apply this attribute to your test method to specify a category.
/// </summary>
[TraitDiscoverer(TestCategoryDiscoverer.FullName, DiscovererInfo.AssemblyName)]
[AttributeUsage(AttributeTargets.Class)]
public class TestCategoryAttribute : Attribute, ITraitAttribute
{
    public TestCategoryAttribute(TestType type)
    {
        this.Type = type;
    }

    public TestType Type { get; }
}

/// <summary>
/// This class discovers all of the tests and test classes that have
/// applied the Category attribute.
/// </summary>
public class TestCategoryDiscoverer : ITraitDiscoverer
{
    public const string FullName = $"Answer.King.Test.Common.CustomTraits.{nameof(TestCategoryDiscoverer)}";

    /// <summary>
    /// Gets the trait values from the Category attribute.
    /// </summary>
    /// <param name="traitAttribute">The trait attribute containing the trait values.</param>
    /// <returns>The trait values.</returns>
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var categoryType = traitAttribute.GetNamedArgument<TestType>("Type");
        yield return new KeyValuePair<string, string>("Category", $"{categoryType}");
    }
}
