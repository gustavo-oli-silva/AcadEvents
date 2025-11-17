namespace AcadEvents.Models;

public class Referencia : DefaultModel
{
    public string Autores { get; set; }
    public string Titulo { get; set; }
    public int Ano { get; set; }
    public string Publicacao { get; set; }
    public string Volume { get; set; }
    public string Paginas { get; set; }
    public string FormatoABNT { get; set; }

    // Relacionamento Inverso: 1 Referencia -> 1 Submissao
    public long SubmissaoId { get; set; }
    public Submissao Submissao { get; set; }

    // Relacionamento: 1 Referencia -> 0..1 DOI
    public long? DOIId { get; set; }
    public DOI? DOI { get; set; }
}