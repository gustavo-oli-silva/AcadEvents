using AcadEvents.Dtos.Orcid;

namespace AcadEvents.Services;

public interface IOrcidClient
{
    /// <summary>
    /// Gera a URL de autorização OAuth para redirecionar o usuário ao ORCID
    /// </summary>
    Task<string> GetAuthorizationUrlAsync(string? state = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Troca o código de autorização por um token de acesso
    /// </summary>
    Task<OrcidTokenResponseDTO?> ExchangeCodeForTokenAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Renova o token de acesso usando o refresh token
    /// </summary>
    Task<OrcidTokenResponseDTO?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém as informações da pessoa autenticada
    /// </summary>
    Task<OrcidPersonDTO?> GetPersonAsync(string accessToken, string orcidId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém o registro completo do ORCID
    /// </summary>
    Task<OrcidRecordDTO?> GetRecordAsync(string accessToken, string orcidId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoga o token de acesso
    /// </summary>
    Task<bool> RevokeTokenAsync(string token, string tokenTypeHint = "access_token", CancellationToken cancellationToken = default);
}

