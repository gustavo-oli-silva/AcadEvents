using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class SessaoRepository : BaseRepository<Sessao>
{
    public SessaoRepository(DbContext db) : base(db) { }
}

