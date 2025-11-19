using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AcadEvents.Dtos.Orcid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AcadEvents.Services;

public class OrcidClient : IOrcidClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrcidClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    private string ClientId => _configuration["Orcid:ClientId"] ?? string.Empty;
    private string ClientSecret => _configuration["Orcid:ClientSecret"] ?? string.Empty;
    private string AuthorizationEndpoint => _configuration["Orcid:AuthorizationEndpoint"] ?? "https://sandbox.orcid.org/oauth/authorize";
    private string TokenEndpoint => _configuration["Orcid:TokenEndpoint"] ?? "https://sandbox.orcid.org/oauth/token";
    private string ApiBaseUrl => _configuration["Orcid:ApiBaseUrl"] ?? "https://sandbox.orcid.org/v3.0";
    private string RedirectUri => _configuration["Orcid:RedirectUri"] ?? string.Empty;
    private string Scopes => _configuration["Orcid:Scopes"] ?? "/read-limited /authenticate";

    public OrcidClient(HttpClient httpClient, IConfiguration configuration, ILogger<OrcidClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public Task<string> GetAuthorizationUrlAsync(string? state = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "client_id", ClientId },
            { "response_type", "code" },
            { "scope", Scopes },
            { "redirect_uri", RedirectUri }
        };

        if (!string.IsNullOrWhiteSpace(state))
        {
            queryParams.Add("state", state);
        }

        var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        var authUrl = $"{AuthorizationEndpoint}?{queryString}";

        _logger.LogInformation("URL de autorização ORCID gerada");
        return Task.FromResult(authUrl);
    }

    public async Task<OrcidTokenResponseDTO?> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            _logger.LogWarning("Código de autorização fornecido é nulo ou vazio");
            return null;
        }

        try
        {
            _logger.LogInformation("Trocando código de autorização por token de acesso");

            var requestBody = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", RedirectUri }
            };

            var content = new FormUrlEncodedContent(requestBody);
            var response = await _httpClient.PostAsync(TokenEndpoint, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Falha ao trocar código por token. Status: {StatusCode}, Erro: {Error}", 
                    response.StatusCode, errorContent);
                return null;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<OrcidTokenResponseDTO>(_jsonOptions, cancellationToken);
            
            if (tokenResponse == null)
            {
                _logger.LogWarning("Resposta do token é nula");
                return null;
            }

            _logger.LogInformation("Token de acesso obtido com sucesso para ORCID: {Orcid}", tokenResponse.Orcid);
            return tokenResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de requisição HTTP ao trocar código por token");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout ao trocar código por token");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao trocar código por token");
            return null;
        }
    }

    public async Task<OrcidTokenResponseDTO?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            _logger.LogWarning("Refresh token fornecido é nulo ou vazio");
            return null;
        }

        try
        {
            _logger.LogInformation("Renovando token de acesso");

            var requestBody = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var content = new FormUrlEncodedContent(requestBody);
            var response = await _httpClient.PostAsync(TokenEndpoint, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Falha ao renovar token. Status: {StatusCode}, Erro: {Error}", 
                    response.StatusCode, errorContent);
                return null;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<OrcidTokenResponseDTO>(_jsonOptions, cancellationToken);
            
            if (tokenResponse == null)
            {
                _logger.LogWarning("Resposta do refresh token é nula");
                return null;
            }

            _logger.LogInformation("Token renovado com sucesso");
            return tokenResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de requisição HTTP ao renovar token");
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout ao renovar token");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao renovar token");
            return null;
        }
    }

    public async Task<OrcidPersonDTO?> GetPersonAsync(string accessToken, string orcidId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(orcidId))
        {
            _logger.LogWarning("Access token ou ORCID ID fornecido é nulo ou vazio");
            return null;
        }

        try
        {
            _logger.LogInformation("Buscando informações da pessoa para ORCID: {Orcid}", orcidId);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/{orcidId}/person");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Falha ao buscar informações da pessoa. Status: {StatusCode}, Erro: {Error}", 
                    response.StatusCode, errorContent);
                return null;
            }

            var personResponse = await response.Content.ReadFromJsonAsync<OrcidPersonResponseDTO>(_jsonOptions, cancellationToken);
            var person = personResponse?.Person;

            if (person == null)
            {
                _logger.LogWarning("Resposta da pessoa é nula para ORCID: {Orcid}", orcidId);
                return null;
            }

            _logger.LogInformation("Informações da pessoa obtidas com sucesso para ORCID: {Orcid}", orcidId);
            return person;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de requisição HTTP ao buscar informações da pessoa para ORCID: {Orcid}", orcidId);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout ao buscar informações da pessoa para ORCID: {Orcid}", orcidId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar informações da pessoa para ORCID: {Orcid}", orcidId);
            return null;
        }
    }

    public async Task<OrcidRecordDTO?> GetRecordAsync(string accessToken, string orcidId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(orcidId))
        {
            _logger.LogWarning("Access token ou ORCID ID fornecido é nulo ou vazio");
            return null;
        }

        try
        {
            _logger.LogInformation("Buscando registro completo para ORCID: {Orcid}", orcidId);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/{orcidId}/record");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Falha ao buscar registro completo. Status: {StatusCode}, Erro: {Error}", 
                    response.StatusCode, errorContent);
                return null;
            }

            var record = await response.Content.ReadFromJsonAsync<OrcidRecordDTO>(_jsonOptions, cancellationToken);

            if (record == null)
            {
                _logger.LogWarning("Resposta do registro é nula para ORCID: {Orcid}", orcidId);
                return null;
            }

            _logger.LogInformation("Registro completo obtido com sucesso para ORCID: {Orcid}", orcidId);
            return record;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de requisição HTTP ao buscar registro completo para ORCID: {Orcid}", orcidId);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout ao buscar registro completo para ORCID: {Orcid}", orcidId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar registro completo para ORCID: {Orcid}", orcidId);
            return null;
        }
    }

    public async Task<bool> RevokeTokenAsync(string token, string tokenTypeHint = "access_token", CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("Token fornecido para revogação é nulo ou vazio");
            return false;
        }

        try
        {
            _logger.LogInformation("Revogando token ORCID");

            var requestBody = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "token", token },
                { "token_type_hint", tokenTypeHint }
            };

            var content = new FormUrlEncodedContent(requestBody);
            var response = await _httpClient.PostAsync(TokenEndpoint, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Falha ao revogar token. Status: {StatusCode}, Erro: {Error}", 
                    response.StatusCode, errorContent);
                return false;
            }

            _logger.LogInformation("Token revogado com sucesso");
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro de requisição HTTP ao revogar token");
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout ao revogar token");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao revogar token");
            return false;
        }
    }
}

