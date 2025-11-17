using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class PerfilORCIDRepository : BaseRepository<PerfilORCID>
{
    public PerfilORCIDRepository(DbContext db) : base(db) { }
}

