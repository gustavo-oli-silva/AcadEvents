using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class SubmissaoRepository : BaseRepository<Submissao>
{
    public SubmissaoRepository(DbContext db) : base(db) { }
}

