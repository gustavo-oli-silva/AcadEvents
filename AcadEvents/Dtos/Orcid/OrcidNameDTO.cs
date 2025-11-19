namespace AcadEvents.Dtos.Orcid;

/// <summary>
/// DTO representando o nome de uma pessoa na API ORCID v3.0
/// </summary>
public record OrcidNameDTO
{
    /// <summary>
    /// Nome fornecido (primeiro nome)
    /// </summary>
    public OrcidString? GivenNames { get; init; }

    /// <summary>
    /// Sobrenome (nome de família)
    /// </summary>
    public OrcidString? FamilyName { get; init; }

    /// <summary>
    /// Crédito do nome (como deve ser exibido)
    /// </summary>
    public OrcidString? CreditName { get; init; }
}

/// <summary>
/// DTO representando um campo de texto no formato ORCID (com visibilidade)
/// </summary>
public record OrcidString
{
    public string? Value { get; init; }
    public string? Visibility { get; init; }
}

