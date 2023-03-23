namespace Answer.King.Domain.Repositories;

public interface IBaseRepository
{
    void BeginTransaction();

    void CommitTransaction();

    void RollbackTransaction();
}
