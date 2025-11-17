using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class EventoRepository : BaseRepository<Evento>
{
    public EventoRepository(DbContext db) : base(db) { }
}

