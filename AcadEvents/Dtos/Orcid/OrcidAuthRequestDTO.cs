namespace AcadEvents.Dtos.Orcid;

/// <summary>
/// DTO opcional para requisição de autenticação ORCID
/// Pode ser usado para customizar o fluxo OAuth (ex: state parameter)
/// </summary>
public record OrcidAuthRequestDTO
{
    /// <summary>
    /// Estado/código de estado para proteção CSRF (opcional)
    /// Será retornado no callback após autorização
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    /// URI de redirecionamento customizada (opcional)
    /// Se não fornecido, será usado o valor da configuração
    /// </summary>
    public string? RedirectUri { get; init; }

    /// <summary>
    /// Escopos customizados (opcional)
    /// Se não fornecido, serão usados os escopos da configuração
    /// </summary>
    public string? Scopes { get; init; }
}

