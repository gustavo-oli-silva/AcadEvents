using AcadEvents.Data;
using AcadEvents.Models;
using Microsoft.EntityFrameworkCore;

namespace AcadEvents.Repositories;

public class SubmissaoRepository : BaseRepository<Submissao>
{
    public SubmissaoRepository(AcadEventsDbContext db) : base(db) { }

    public async Task<Submissao?> FindByIdWithEventoAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.Submissoes
            .Include(s => s.Evento)
            .Include(s => s.TrilhaTematica)
                .ThenInclude(tt => tt.Trilha)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}

