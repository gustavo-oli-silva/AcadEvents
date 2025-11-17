using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class AutorRepository : BaseRepository<Autor>
{
    public AutorRepository(DbContext db) : base(db) { }
}

