using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class UsuarioRepository : BaseRepository<Usuario>
{
    public UsuarioRepository(DbContext db) : base(db) { }
}

