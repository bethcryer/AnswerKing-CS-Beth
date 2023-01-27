using LiteDB;

namespace Answer.King.Infrastructure;

public interface ILiteDbConnectionFactory
{
    ILiteDatabase GetConnection();
}
