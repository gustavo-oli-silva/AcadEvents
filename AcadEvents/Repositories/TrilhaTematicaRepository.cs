using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class TrilhaTematicaRepository : BaseRepository<TrilhaTematica>
{
    public TrilhaTematicaRepository(DbContext db) : base(db) { }
}

