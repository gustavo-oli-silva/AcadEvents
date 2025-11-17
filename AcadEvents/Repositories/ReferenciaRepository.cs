using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class ReferenciaRepository : BaseRepository<Referencia>
{
    public ReferenciaRepository(DbContext db) : base(db) { }
}

