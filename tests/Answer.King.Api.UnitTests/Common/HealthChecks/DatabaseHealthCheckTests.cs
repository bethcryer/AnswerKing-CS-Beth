using Answer.King.Api.Common.HealthChecks;
using Answer.King.Domain.Inventory;
using Answer.King.Domain.Inventory.Models;
using Answer.King.Infrastructure;
using Answer.King.Test.Common.CustomTraits;
using LiteDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Answer.King.Api.UnitTests.Common.HealthChecks;

[TestCategory(TestType.Unit)]
public class DatabaseHealthCheckTests
{
    private readonly ILiteDbConnectionFactory dbConnectionFactory = Substitute.For<ILiteDbConnectionFactory>();
    private readonly ILiteDatabase liteDb = Substitute.For<ILiteDatabase>();
    private readonly ILiteCollection<Category> liteCollection = Substitute.For<ILiteCollection<Category>>();

    [Fact]
    public async void CheckHealthAsync_DelayUnderDegradedThreshold_ReturnsHealthCheckResultHealthy()
    {
        // Arrange
        var options = Options.Create(new HealthCheckOptions());

        this.dbConnectionFactory.GetConnection().Returns(this.liteDb);
        this.liteDb.GetCollection<Category>().Returns(this.liteCollection);
        this.liteCollection.FindOne(c => true).Returns(new Category("name", "desc", new List<ProductId>()));

        var dbHealthCheck = new DatabaseHealthCheck(this.dbConnectionFactory, options);

        // Act
        var result = await dbHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.IsType<HealthCheckResult>(result);
        Assert.Equal(result, HealthCheckResult.Healthy("Healthy result from DatabaseHealthCheck"));
    }

    [Fact]
    public async void CheckHealthAsync_DelayOverDegradedThreshold_ReturnsHealthCheckResultDegraded()
    {
        // Arrange
        var options = Options.Create(new HealthCheckOptions { DegradedThresholdMs = 0 });

        this.dbConnectionFactory.GetConnection().Returns(this.liteDb);
        this.liteDb.GetCollection<Category>().Returns(this.liteCollection);
        this.liteCollection.FindOne(c => true).Returns(new Category("name", "desc", new List<ProductId>()));

        var dbHealthCheck = new DatabaseHealthCheck(this.dbConnectionFactory, options);

        // Act
        var result = await dbHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.IsType<HealthCheckResult>(result);
        Assert.Equal(result, HealthCheckResult.Degraded("Degraded result from DatabaseHealthCheck"));
    }

    [Fact]
    public async void CheckHealthAsync_DelayOverUnhealthyThreshold_ReturnsHealthCheckResultUnhealthy()
    {
        // Arrange
        var options = Options.Create(new HealthCheckOptions { DegradedThresholdMs = 0, UnhealthyThresholdMs = 0 });

        this.dbConnectionFactory.GetConnection().Returns(this.liteDb);
        this.liteDb.GetCollection<Category>().Returns(this.liteCollection);
        this.liteCollection.FindOne(c => true).Returns(new Category("name", "desc", new List<ProductId>()));

        var dbHealthCheck = new DatabaseHealthCheck(this.dbConnectionFactory, options);

        // Act
        var result = await dbHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.IsType<HealthCheckResult>(result);
        Assert.Equal(result, HealthCheckResult.Unhealthy("Unhealthy result from DatabaseHealthCheck"));
    }
}
