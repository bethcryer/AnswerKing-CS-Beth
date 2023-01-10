using LiteDB;
using Microsoft.Extensions.Configuration;

namespace Answer.King.Infrastructure;

public class LiteDbConnectionFactory : ILiteDbConnectionFactory
{
    public LiteDbConnectionFactory(IConfiguration config, BsonMapper mapper)
    {
        var connectionString = config.GetConnectionString("AnswerKing");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new LiteDbConnectionFactoryException(
                "Cannot find database connection string in configuration file.");
        }

        this.Database = new LiteDatabase(connectionString, mapper);
    }

    private LiteDatabase Database { get; }

    public LiteDatabase GetConnection()
    {
        return this.Database;
    }
}
