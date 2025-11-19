namespace AcadEvents.Dtos.Orcid;

/// <summary>
/// DTO representando os dados de uma pessoa (person) na API ORCID v3.0
/// Baseado na estrutura do modelo ORCID v3.0
/// </summary>
public record OrcidPersonDTO
{
    /// <summary>
    /// Informações do nome da pessoa
    /// </summary>
    public OrcidNameDTO? Name { get; init; }

    /// <summary>
    /// Biografia da pessoa
    /// </summary>
    public OrcidBiographyDTO? Biography { get; init; }

    /// <summary>
    /// País da residência primária
    /// </summary>
    public OrcidAddressDTO? Address { get; init; }

    /// <summary>
    /// Palavras-chave/research domains
    /// </summary>
    public List<OrcidKeywordDTO>? Keywords { get; init; }

    /// <summary>
    /// URLs externas e websites
    /// </summary>
    public List<OrcidResearcherUrlDTO>? ResearcherUrls { get; init; }

    /// <summary>
    /// Emails do pesquisador
    /// </summary>
    public List<OrcidEmailDTO>? Emails { get; init; }
}

/// <summary>
/// DTO representando biografia na API ORCID
/// </summary>
public record OrcidBiographyDTO
{
    public string? Content { get; init; }
    public string? Visibility { get; init; }
}

/// <summary>
/// DTO representando endereço na API ORCID
/// </summary>
public record OrcidAddressDTO
{
    public List<OrcidCountryDTO>? Country { get; init; }
}

/// <summary>
/// DTO representando país na API ORCID
/// </summary>
public record OrcidCountryDTO
{
    public string? Value { get; init; }
}

/// <summary>
/// DTO representando palavra-chave na API ORCID
/// </summary>
public record OrcidKeywordDTO
{
    public string? Content { get; init; }
    public string? Visibility { get; init; }
}

/// <summary>
/// DTO representando URL de pesquisador na API ORCID
/// </summary>
public record OrcidResearcherUrlDTO
{
    public OrcidResearcherUrlNameDTO? UrlName { get; init; }
    public OrcidString? Url { get; init; }
    public string? Visibility { get; init; }
}

/// <summary>
/// DTO representando nome de URL de pesquisador
/// </summary>
public record OrcidResearcherUrlNameDTO
{
    public string? Content { get; init; }
}

/// <summary>
/// DTO representando email na API ORCID
/// </summary>
public record OrcidEmailDTO
{
    public string? Email { get; init; }
    public bool? Primary { get; init; }
    public bool? Verified { get; init; }
    public string? Visibility { get; init; }
}

