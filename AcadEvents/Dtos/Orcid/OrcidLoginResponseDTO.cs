namespace AcadEvents.Dtos.Orcid;

/// <summary>
/// DTO de resposta após login bem-sucedido com ORCID
/// Retornado ao cliente após autenticação OAuth completa
/// </summary>
public record OrcidLoginResponseDTO
{
    /// <summary>
    /// ORCID ID do usuário autenticado
    /// </summary>
    public string OrcidId { get; init; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Email do usuário (se disponível e visível)
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Indica se o perfil ORCID foi vinculado a um usuário existente ou se foi criado novo usuário
    /// </summary>
    public bool IsNewUser { get; init; }

    /// <summary>
    /// ID do usuário no sistema local (se vinculado ou criado)
    /// </summary>
    public long? UserId { get; init; }

    /// <summary>
    /// Indica se o perfil ORCID está verificado
    /// </summary>
    public bool IsVerified { get; init; }

    /// <summary>
    /// Mensagem de sucesso
    /// </summary>
    public string Message { get; init; } = "Login realizado com sucesso";

    /// <summary>
    /// Token JWT do sistema (se aplicável - para autenticação interna)
    /// </summary>
    public string? JwtToken { get; init; }
}

