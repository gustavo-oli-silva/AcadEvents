using AcadEvents.Dtos.Orcid;

namespace AcadEvents.Services;

public interface IOrcidService
{
    /// <summary>
    /// Retorna a URL de autorização OAuth para redirecionar o usuário
    /// </summary>
    Task<string> GetAuthorizationUrlAsync(string? state = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processa o callback OAuth após autorização do usuário
    /// Cria ou atualiza o usuário e perfil ORCID com os tokens e informações
    /// </summary>
    Task<OrcidLoginResponseDTO?> HandleCallbackAsync(string code, string? state = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Vincula um perfil ORCID a um usuário existente no sistema
    /// </summary>
    Task<OrcidLoginResponseDTO?> LinkOrcidToUserAsync(long userId, string orcidId, string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sincroniza os dados do perfil ORCID com o perfil local do usuário
    /// </summary>
    Task<bool> SyncUserProfileAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna as informações ORCID de um usuário
    /// </summary>
    Task<OrcidUserInfoDTO?> GetUserOrcidAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoga o token ORCID de um usuário (logout)
    /// </summary>
    Task<bool> RevokeUserTokenAsync(long userId, CancellationToken cancellationToken = default);
}

