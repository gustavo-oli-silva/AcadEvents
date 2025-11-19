namespace AcadEvents.Dtos.Orcid;

/// <summary>
/// DTO representando a resposta da API ORCID ao trocar código de autorização por token de acesso.
/// Baseado na documentação: https://info.orcid.org/pt/mãos-à-obra-com-orcid-api/2-coletar-autenticado-orcid-IDs-e-permissões/
/// </summary>
public record OrcidTokenResponseDTO
{
    /// <summary>
    /// Token de acesso usado para fazer chamadas autenticadas à API ORCID
    /// </summary>
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>
    /// Tipo do token (geralmente "bearer")
    /// </summary>
    public string TokenType { get; init; } = "bearer";

    /// <summary>
    /// Token de atualização usado para renovar o access token quando expirar
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Tempo de expiração do token em segundos
    /// </summary>
    public int ExpiresIn { get; init; }

    /// <summary>
    /// Escopos concedidos pelo usuário, separados por espaço
    /// </summary>
    public string? Scope { get; init; }

    /// <summary>
    /// ORCID ID do usuário autenticado (formato: 0000-0000-0000-0000)
    /// </summary>
    public string Orcid { get; init; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário conforme registrado no ORCID
    /// </summary>
    public string? Name { get; init; }
}

