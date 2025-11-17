namespace AcadEvents.Repositories;

public interface IRepository<T> where T : class
{
    Task<List<T>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<T?> FindByIdAsync(long id, CancellationToken cancellationToken = default);
}