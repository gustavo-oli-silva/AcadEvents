using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class ComiteCientificoRepository : BaseRepository<ComiteCientifico>
{
    public ComiteCientificoRepository(DbContext db) : base(db) { }
}

