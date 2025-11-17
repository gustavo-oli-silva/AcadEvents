using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class TrilhaRepository : BaseRepository<Trilha>
{
    public TrilhaRepository(DbContext db) : base(db) { }
}

