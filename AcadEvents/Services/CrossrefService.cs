using System.Net.Http.Json;
using System.Text.Json;
using AcadEvents.Dtos;
using Microsoft.Extensions.Logging;

namespace AcadEvents.Services;

public class CrossrefService : ICrossrefService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CrossrefService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public CrossrefService(HttpClient httpClient, ILogger<CrossrefService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<CrossrefWorkDTO?> GetWorkByDoiAsync(string doi, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(doi))
        {
            _logger.LogWarning("DOI fornecido é nulo ou vazio");
            return null;
        }

        try
        {
            // Remove espaços e caracteres especiais do DOI
            var cleanDoi = doi.Trim();
            
            _logger.LogInformation("Buscando work no Crossref para DOI: {DOI}", cleanDoi);

            var response = await _httpClient.GetAsync($"works/{cleanDoi}", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao buscar work no Crossref. Status: {StatusCode}, DOI: {DOI}", 
                    response.StatusCode, cleanDoi);
                return null;
            }

            var crossrefResponse = await response.Content.ReadFromJsonAsync<CrossrefResponseDTO>(_jsonOptions, cancellationToken);
            
            if (crossrefResponse?.Message == null)
            {
                _logger.LogWarning("Resposta do Crossref não contém dados válidos para DOI: {DOI}", cleanDoi);
                return null;
            }

            var work = MapToWorkDTO(crossrefResponse.Message);
            _logger.LogInformation("Work obtido com sucesso do Crossref para DOI: {DOI}", cleanDoi);
            
            return work;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de requisição HTTP ao buscar work no Crossref para DOI: {DOI}", doi);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout ao buscar work no Crossref para DOI: {DOI}", doi);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar work no Crossref para DOI: {DOI}", doi);
            return null;
        }
    }

    private static CrossrefWorkDTO MapToWorkDTO(CrossrefMessageDTO message)
    {
        var authors = message.Author?
            .Select(a => $"{a.Given} {a.Family}".Trim())
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .ToList() ?? new List<string>();

        var title = message.Title?.FirstOrDefault() ?? string.Empty;
        var containerTitle = message.ContainerTitle?.FirstOrDefault() ?? string.Empty;
        var url = message.URL?.FirstOrDefault() ?? string.Empty;

        var publishedPrint = message.PublishedPrint?.DateParts?.FirstOrDefault();
        var publishedOnline = message.PublishedOnline?.DateParts?.FirstOrDefault();

        var publishedPrintStr = publishedPrint != null && publishedPrint.Count >= 1
            ? string.Join("-", publishedPrint.Take(3).Select(d => d.ToString().PadLeft(2, '0')))
            : null;

        var publishedOnlineStr = publishedOnline != null && publishedOnline.Count >= 1
            ? string.Join("-", publishedOnline.Take(3).Select(d => d.ToString().PadLeft(2, '0')))
            : null;

        return new CrossrefWorkDTO
        {
            DOI = message.DOI,
            Title = title,
            Author = authors,
            Publisher = message.Publisher,
            ContainerTitle = containerTitle,
            PublishedPrint = publishedPrintStr,
            PublishedOnline = publishedOnlineStr,
            Type = message.Type,
            URL = url,
            Abstract = message.Abstract
        };
    }
}

