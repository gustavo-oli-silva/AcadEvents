using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class AvaliacaoRepository : BaseRepository<Avaliacao>
{
    public AvaliacaoRepository(DbContext db) : base(db) { }
}

