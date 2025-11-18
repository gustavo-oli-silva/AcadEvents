using AcadEvents.Models;

namespace AcadEvents.Dtos;

public record EventoResponseDTO
{
    public long Id { get; init; }
    public string Nome { get; init; }
    public string Descricao { get; init; }
    public DateTime DataInicio { get; init; }
    public DateTime DataFim { get; init; }
    public string Local { get; init; }
    public string Site { get; init; }
    public string Logo { get; init; }
    public string StatusEvento { get; init; }
    public long ConfiguracaoEventoId { get; init; }
    public List<long> OrganizadoresIds { get; init; } = new();

    public static EventoResponseDTO ValueOf(Evento evento)
    {
        return new EventoResponseDTO
        {
            Id = evento.Id,
            Nome = evento.Nome,
            Descricao = evento.Descricao,
            DataInicio = evento.DataInicio,
            DataFim = evento.DataFim,
            Local = evento.Local,
            Site = evento.Site,
            Logo = evento.Logo,
            StatusEvento = evento.StatusEvento,
            ConfiguracaoEventoId = evento.ConfiguracaoEventoId,
            OrganizadoresIds = evento.Organizadores?.Select(o => o.Id).ToList() ?? new List<long>()
        };
    }
}

