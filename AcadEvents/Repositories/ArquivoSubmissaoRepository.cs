using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class ArquivoSubmissaoRepository : BaseRepository<ArquivoSubmissao>
{
    public ArquivoSubmissaoRepository(DbContext db) : base(db) { }
}

