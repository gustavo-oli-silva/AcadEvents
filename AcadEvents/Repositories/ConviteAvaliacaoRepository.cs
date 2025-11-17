using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class ConviteAvaliacaoRepository : BaseRepository<ConviteAvaliacao>
{
    public ConviteAvaliacaoRepository(DbContext db) : base(db) { }
}

