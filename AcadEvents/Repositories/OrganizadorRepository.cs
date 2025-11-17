using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class OrganizadorRepository : BaseRepository<Organizador>
{
    public OrganizadorRepository(DbContext db) : base(db) { }
}

