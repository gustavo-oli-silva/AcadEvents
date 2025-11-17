using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class DOIRepository : BaseRepository<DOI>
{
    public DOIRepository(DbContext db) : base(db) { }
}

