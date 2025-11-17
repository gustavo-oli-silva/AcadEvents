using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class AvaliadorRepository : BaseRepository<Avaliador>
{
    public AvaliadorRepository(DbContext db) : base(db) { }
}

