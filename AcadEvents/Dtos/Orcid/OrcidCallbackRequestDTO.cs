namespace AcadEvents.Dtos.Orcid;

/// <summary>
/// DTO representando os parâmetros recebidos no callback do ORCID após autorização
/// </summary>
public record OrcidCallbackRequestDTO
{
    /// <summary>
    /// Código de autorização fornecido pelo ORCID
    /// Este código será trocado por um token de acesso
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Estado/código de estado (se fornecido na requisição inicial)
    /// Usado para proteção CSRF
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    /// Código de erro (se houver erro na autorização)
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Descrição do erro (se houver)
    /// </summary>
    public string? ErrorDescription { get; init; }
}

