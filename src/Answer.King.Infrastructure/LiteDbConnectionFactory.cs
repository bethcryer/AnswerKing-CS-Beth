using System;
using LiteDB;
using Microsoft.Extensions.Configuration;

namespace Answer.King.Infrastructure;

public class LiteDbConnectionFactory : ILiteDbConnectionFactory
{
    public LiteDbConnectionFactory(IConfiguration config)
    {
        var connectionString = config.GetConnectionString("AnswerKing");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new LiteDbConnectionFactoryException(
                "Cannot find database connection string in configuration file.");
        }

        this.Database = new LiteDatabase(connectionString);
    }

    private LiteDatabase Database { get; }

    public LiteDatabase GetConnection()
    {
        return this.Database;
    }
}

[Serializable]
public class LiteDbConnectionFactoryException : Exception
{
    public LiteDbConnectionFactoryException(string message) : base(message)
    {
    }

    public LiteDbConnectionFactoryException() : base()
    {
    }

    public LiteDbConnectionFactoryException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
