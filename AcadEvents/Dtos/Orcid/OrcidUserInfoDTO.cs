namespace AcadEvents.Dtos.Orcid;

/// <summary>
/// DTO simplificado com informações essenciais do usuário ORCID
/// Usado para facilitar o mapeamento para o modelo PerfilORCID
/// </summary>
public record OrcidUserInfoDTO
{
    /// <summary>
    /// ORCID ID do usuário (formato: 0000-0000-0000-0000)
    /// </summary>
    public string OrcidId { get; init; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário
    /// </summary>
    public string? FullName { get; init; }

    /// <summary>
    /// Primeiro nome
    /// </summary>
    public string? GivenName { get; init; }

    /// <summary>
    /// Sobrenome
    /// </summary>
    public string? FamilyName { get; init; }

    /// <summary>
    /// Email primário (se disponível e visível)
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Biografia/resumo
    /// </summary>
    public string? Biography { get; init; }

    /// <summary>
    /// País de residência
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    /// Palavras-chave/research domains
    /// </summary>
    public List<string> Keywords { get; init; } = new();

    /// <summary>
    /// URLs de pesquisador (websites, perfis)
    /// </summary>
    public List<string> ResearcherUrls { get; init; } = new();

    /// <summary>
    /// Método para mapear de OrcidPersonDTO para OrcidUserInfoDTO
    /// </summary>
    public static OrcidUserInfoDTO FromPersonDTO(string orcidId, OrcidPersonDTO? person)
    {
        if (person == null)
        {
            return new OrcidUserInfoDTO { OrcidId = orcidId };
        }

        var fullName = person.Name?.CreditName?.Value 
            ?? $"{person.Name?.GivenNames?.Value ?? ""} {person.Name?.FamilyName?.Value ?? ""}".Trim();

        var email = person.Emails?
            .FirstOrDefault(e => e.Primary == true || e.Verified == true)?
            .Email;

        var keywords = person.Keywords?
            .Where(k => !string.IsNullOrWhiteSpace(k.Content))
            .Select(k => k.Content!)
            .ToList() ?? new List<string>();

        var urls = person.ResearcherUrls?
            .Where(u => !string.IsNullOrWhiteSpace(u.Url?.Value))
            .Select(u => u.Url!.Value!)
            .ToList() ?? new List<string>();

        var country = person.Address?.Country?.FirstOrDefault()?.Value;

        return new OrcidUserInfoDTO
        {
            OrcidId = orcidId,
            FullName = !string.IsNullOrWhiteSpace(fullName) ? fullName : null,
            GivenName = person.Name?.GivenNames?.Value,
            FamilyName = person.Name?.FamilyName?.Value,
            Email = email,
            Biography = person.Biography?.Content,
            Country = country,
            Keywords = keywords,
            ResearcherUrls = urls
        };
    }
}

