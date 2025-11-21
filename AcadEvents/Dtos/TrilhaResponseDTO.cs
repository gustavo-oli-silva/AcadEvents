using AcadEvents.Models;

namespace AcadEvents.Dtos;

public record TrilhaResponseDTO
{
    public long Id { get; init; }
    public string Nome { get; init; }
    public string Descricao { get; init; }
    public string Coordenador { get; init; }
    public int LimiteSubmissoes { get; init; }
    public List<long> EventosIds { get; init; } = new();

    public static TrilhaResponseDTO ValueOf(Trilha trilha)
    {
        return new TrilhaResponseDTO
        {
            Id = trilha.Id,
            Nome = trilha.Nome,
            Descricao = trilha.Descricao,
            Coordenador = trilha.Coordenador,
            LimiteSubmissoes = trilha.LimiteSubmissoes,
            EventosIds = trilha.Eventos?.Select(e => e.Id).ToList() ?? new List<long>()
        };
    }
}

