namespace AcadEvents.Dtos;

public record CrossrefWorkDTO
{
    public string? DOI { get; init; }
    public string? Title { get; init; }
    public List<string>? Author { get; init; }
    public string? Publisher { get; init; }
    public string? ContainerTitle { get; init; }
    public string? PublishedPrint { get; init; }
    public string? PublishedOnline { get; init; }
    public string? Type { get; init; }
    public string? URL { get; init; }
    public string? Abstract { get; init; }
}

public record CrossrefResponseDTO
{
    public string Status { get; init; } = string.Empty;
    public CrossrefMessageDTO? Message { get; init; }
}

public record CrossrefMessageDTO
{
    public string? DOI { get; init; }
    public List<string>? Title { get; init; }
    public List<CrossrefAuthorDTO>? Author { get; init; }
    public string? Publisher { get; init; }
    public List<string>? ContainerTitle { get; init; }
    public CrossrefDateDTO? PublishedPrint { get; init; }
    public CrossrefDateDTO? PublishedOnline { get; init; }
    public string? Type { get; init; }
    public List<string>? URL { get; init; }
    public string? Abstract { get; init; }
}

public record CrossrefAuthorDTO
{
    public string? Given { get; init; }
    public string? Family { get; init; }
    public string? Sequence { get; init; }
}

public record CrossrefDateDTO
{
    public List<List<int>>? DateParts { get; init; }
}

