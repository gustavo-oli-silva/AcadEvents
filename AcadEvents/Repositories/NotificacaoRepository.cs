using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class NotificacaoRepository : BaseRepository<Notificacao>
{
    public NotificacaoRepository(DbContext db) : base(db) { }
}

