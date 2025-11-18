namespace AcadEvents.Models;

public class Trilha : DefaultModel
{
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public string Coordenador { get; set; }
    public int LimiteSubmissoes { get; set; }

    // Relacionamento Inverso: 1 Trilha -> 1 Evento
    public long EventoId { get; set; }
    public Evento Evento { get; set; }

    // Relacionamento: 1 Trilha -> * TrilhaTematica
    public ICollection<TrilhaTematica> Tematicas { get; set; } = new List<TrilhaTematica>();

    // Relacionamento: 1 Trilha -> * Sessao
    public ICollection<Sessao> Sessoes { get; set; } = new List<Sessao>();

}