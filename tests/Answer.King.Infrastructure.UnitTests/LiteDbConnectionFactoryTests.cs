namespace Answer.King.Infrastructure.UnitTests;

using Answer.King.Test.Common.CustomTraits;
using LiteDB;
using Microsoft.Extensions.Configuration;

[TestCategory(TestType.Unit)]
public class LiteDbConnectionFactoryTests : IDisposable
{
    public LiteDbConnectionFactoryTests()
    {
        this.TestDbName = $"Answer.King.{Guid.NewGuid()}.db";
    }

    private string TestDbName { get; }

    [Fact]
    public void Constructor_ConnectionStringIsNullOrWhiteSpace_ThrowsLiteDbConnectionFactoryException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build();
        var mapper = new BsonMapper();

        // Act / Assert
        Assert.Throws<LiteDbConnectionFactoryException>(
            () => new LiteDbConnectionFactory(configuration, mapper));
    }

    [Fact]
    public void Constructor_ValidArguments_ReturnsInstance()
    {
        // Arrange
        var configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:AnswerKing", $"filename={this.TestDbName};Connection=Shared;" },
                })
                .Build();

        var mapper = new BsonMapper();

        // Act
        LiteDbConnectionFactory? instance = null;
        var exception = Record.Exception(() => instance = new LiteDbConnectionFactory(configuration, mapper));

        // Assert
        Assert.NotNull(instance);
        Assert.Null(exception);
    }

    [Fact]
    public void GetConnection_ReturnsLiteDatabase()
    {
        // Arrange
        var configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:AnswerKing", $"filename={this.TestDbName};Connection=Shared;" },
                })
                .Build();

        var mapper = new BsonMapper();
        var sut = new LiteDbConnectionFactory(configuration, mapper);

        // Act
        var connection = sut.GetConnection();

        // Assert
        Assert.NotNull(connection);
        Assert.IsType<LiteDatabase>(connection);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            File.Delete($".\\{this.TestDbName}");
        }
    }
}
