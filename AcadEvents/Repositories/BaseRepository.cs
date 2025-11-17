using Microsoft.EntityFrameworkCore;

namespace AcadEvents.Repositories;

public class BaseRepository<T> : IRepository<T> where T : class
{
    private readonly DbContext _db;

    public BaseRepository(DbContext db) { _db = db; }

    public Task<List<T>> FindAllAsync(CancellationToken cancellationToken = default)
        => _db.Set<T>().ToListAsync(cancellationToken);

    public async Task<T?> FindByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        // Agora o .Set<T>() sabe que T é uma classe e vai compilar
        var found = await _db.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        return found;
    }
}