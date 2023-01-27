namespace Answer.King.Domain;

public interface IAggregateRepository<T>
    where T : IAggregateRoot
{
    Task<IEnumerable<T>> GetAll();

    Task<T?> GetOne(long id);

    Task Save(T item);
}
