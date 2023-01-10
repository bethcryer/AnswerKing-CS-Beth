namespace Answer.King.Api.IntegrationTests.Common;

public class WebFixtures : IAsyncLifetime
{
    private readonly string testDbName = $"Answer.King.{Guid.NewGuid()}.db";

    public IAlbaHost AlbaHost { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        this.AlbaHost = await Alba.AlbaHost.For<Program>(
            hostBuilder => hostBuilder.UseSetting("ConnectionStrings:AnswerKing", $"filename={this.testDbName};Connection=Shared;"));
    }

    public async Task DisposeAsync()
    {
        await this.AlbaHost.DisposeAsync();
        File.Delete($".\\{this.testDbName}");
    }
}
