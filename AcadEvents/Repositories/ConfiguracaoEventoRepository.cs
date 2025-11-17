using Microsoft.EntityFrameworkCore;
using AcadEvents.Models;

namespace AcadEvents.Repositories;

public class ConfiguracaoEventoRepository : BaseRepository<ConfiguracaoEvento>
{
    public ConfiguracaoEventoRepository(DbContext db) : base(db) { }
}

