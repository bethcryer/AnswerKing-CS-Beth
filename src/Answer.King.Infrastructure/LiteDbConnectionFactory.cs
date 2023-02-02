using LiteDB;
using Microsoft.Extensions.Configuration;

namespace Answer.King.Infrastructure;

public class LiteDbConnectionFactory : ILiteDbConnectionFactory
{
    public LiteDbConnectionFactory(IConfiguration config, BsonMapper mapper)
    {
        System.IO.Directory.CreateDirectory("db");
        var connectionString = config.GetConnectionString("AnswerKing");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new LiteDbConnectionFactoryException(
                "Cannot find database connection string in configuration file.");
        }

        this.Database = new LiteDatabase(connectionString, mapper);
    }

    private ILiteDatabase Database { get; }

    public ILiteDatabase GetConnection()
    {
        return this.Database;
    }
}
