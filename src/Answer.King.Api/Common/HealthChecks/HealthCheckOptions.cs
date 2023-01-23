namespace Answer.King.Api.Common.HealthChecks;

public class HealthCheckOptions
{
    public const string OptionsConfig = "HealthCheck";

    public long DegradedThresholdMs { get; set; } = 100;

    public long UnhealthyThresholdMs { get; set; } = 200;
}
