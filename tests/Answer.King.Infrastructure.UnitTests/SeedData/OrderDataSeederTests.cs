using System.Reflection;
using Answer.King.Infrastructure.SeedData;
using Answer.King.Test.Common.CustomTraits;
using NSubstitute;

namespace Answer.King.Infrastructure.UnitTests.SeedData;

[TestCategory(TestType.Unit)]
public class OrderDataSeederTests
{
    private readonly ILiteDbConnectionFactory dbConnectionFactory = Substitute.For<ILiteDbConnectionFactory>();

    [Fact]
    public void SeedData_DataAlreadySeeded_Returns()
    {
        var orderDataSeeder = new OrderDataSeeder();

        var dataSeededFieldInfo =
            typeof(OrderDataSeeder).GetProperty("DataSeeded", BindingFlags.Instance | BindingFlags.NonPublic);

        dataSeededFieldInfo?.SetValue(orderDataSeeder, true);

        orderDataSeeder.SeedData(this.dbConnectionFactory);

        this.dbConnectionFactory.DidNotReceive().GetConnection();
    }
}
