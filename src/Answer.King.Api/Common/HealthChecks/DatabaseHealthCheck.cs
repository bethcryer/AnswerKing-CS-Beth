using System.Diagnostics;
using Answer.King.Domain.Inventory;
using Answer.King.Infrastructure;
using LiteDB;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Answer.King.Api.Common.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ILiteDatabase liteDB;
    private readonly HealthCheckOptions options;

    public DatabaseHealthCheck(ILiteDbConnectionFactory connections, IOptions<HealthCheckOptions> options)
    {
        this.liteDB = connections.GetConnection();
        this.options = options.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var startTime = Stopwatch.GetTimestamp();
        await this.QueryDB();
        var responseTime = Stopwatch.GetElapsedTime(startTime);

        if (responseTime.Milliseconds < this.options.DegradedThresholdMs)
        {
            return await Task.FromResult(HealthCheckResult.Healthy("Healthy result from DatabaseHealthCheck"));
        }
        else if (responseTime.Milliseconds < this.options.UnhealthyThresholdMs)
        {
            return await Task.FromResult(HealthCheckResult.Degraded("Degraded result from DatabaseHealthCheck"));
        }

        return await Task.FromResult(HealthCheckResult.Unhealthy("Unhealthy result from DatabaseHealthCheck"));
    }

    public Task<Category> QueryDB()
    {
        var collection = this.liteDB.GetCollection<Category>();
        collection.EnsureIndex("products");

        return Task.FromResult(collection.FindOne(c => true));
    }
}
