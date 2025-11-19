namespace AcadEvents.Dtos.Orcid;

/// <summary>
/// DTO representando um registro completo (record) do ORCID v3.0
/// Esta é a estrutura completa retornada ao fazer GET /v3.0/{orcid}/record
/// </summary>
public record OrcidRecordDTO
{
    /// <summary>
    /// Dados da pessoa
    /// </summary>
    public OrcidPersonDTO? Person { get; init; }

    /// <summary>
    /// ORCID ID do registro
    /// </summary>
    public string? OrcidIdentifier { get; init; }
}

/// <summary>
/// DTO representando apenas os dados básicos da pessoa retornados em /person
/// Esta é a estrutura retornada ao fazer GET /v3.0/{orcid}/person
/// </summary>
public record OrcidPersonResponseDTO
{
    public OrcidPersonDTO? Person { get; init; }
}

