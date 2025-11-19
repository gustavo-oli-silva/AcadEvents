namespace AcadEvents.Dtos.Orcid;

/// <summary>
/// DTO representando erros retornados pela API ORCID
/// </summary>
public record OrcidErrorDTO
{
    /// <summary>
    /// Código do erro
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Descrição detalhada do erro
    /// </summary>
    public string? ErrorDescription { get; init; }

    /// <summary>
    /// URI de referência do erro (se disponível)
    /// </summary>
    public string? ErrorUri { get; init; }
}

