using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class HistoricoEventoRepository : BaseRepository<HistoricoEvento>
{
    public HistoricoEventoRepository(DbContext db) : base(db) { }
}

