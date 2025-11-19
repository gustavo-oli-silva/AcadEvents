using AcadEvents.Data;
using AcadEvents.Dtos.Orcid;
using AcadEvents.Models;
using AcadEvents.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AcadEvents.Services;

public class OrcidService : IOrcidService
{
    private readonly IOrcidClient _orcidClient;
    private readonly UsuarioRepository _usuarioRepository;
    private readonly PerfilORCIDRepository _perfilORCIDRepository;
    private readonly AcadEventsDbContext _dbContext;
    private readonly ILogger<OrcidService> _logger;

    public OrcidService(
        IOrcidClient orcidClient,
        UsuarioRepository usuarioRepository,
        PerfilORCIDRepository perfilORCIDRepository,
        AcadEventsDbContext dbContext,
        ILogger<OrcidService> logger)
    {
        _orcidClient = orcidClient;
        _usuarioRepository = usuarioRepository;
        _perfilORCIDRepository = perfilORCIDRepository;
        _dbContext = dbContext;
        _logger = logger;
    }

    public Task<string> GetAuthorizationUrlAsync(string? state = null, CancellationToken cancellationToken = default)
    {
        return _orcidClient.GetAuthorizationUrlAsync(state, cancellationToken);
    }

    public async Task<OrcidLoginResponseDTO?> HandleCallbackAsync(string code, string? state = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            _logger.LogWarning("Código de autorização fornecido é nulo ou vazio");
            return null;
        }

        try
        {
            // 1. Trocar código por token
            var tokenResponse = await _orcidClient.ExchangeCodeForTokenAsync(code, cancellationToken);
            if (tokenResponse == null)
            {
                _logger.LogWarning("Falha ao trocar código por token");
                return null;
            }

            var orcidId = tokenResponse.Orcid;

            // 2. Buscar informações da pessoa no ORCID
            var person = await _orcidClient.GetPersonAsync(tokenResponse.AccessToken, orcidId, cancellationToken);
            if (person == null)
            {
                _logger.LogWarning("Falha ao buscar informações da pessoa para ORCID: {Orcid}", orcidId);
                return null;
            }

            // 3. Mapear dados do ORCID para DTO simplificado
            var userInfo = OrcidUserInfoDTO.FromPersonDTO(orcidId, person);

            // 4. Verificar se já existe perfil ORCID com este ORCID ID
            var existingPerfil = await _dbContext.PerfisORCID
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.OrcidId == orcidId, cancellationToken);

            Usuario? usuario;
            PerfilORCID perfil;
            bool isNewUser = false;

            if (existingPerfil != null)
            {
                // Atualizar perfil existente
                perfil = await _perfilORCIDRepository.FindByIdAsync(existingPerfil.Id, cancellationToken);
                if (perfil == null)
                {
                    _logger.LogError("Perfil ORCID encontrado mas não pode ser carregado. ID: {Id}", existingPerfil.Id);
                    return null;
                }

                usuario = await _usuarioRepository.FindByIdAsync(perfil.UsuarioId, cancellationToken);
                if (usuario == null)
                {
                    _logger.LogError("Usuário não encontrado para perfil ORCID. UserId: {UserId}", perfil.UsuarioId);
                    return null;
                }

                _logger.LogInformation("Atualizando perfil ORCID existente para ORCID: {Orcid}", orcidId);
            }
            else
            {
                // Verificar se existe usuário com email do ORCID (se disponível)
                if (!string.IsNullOrWhiteSpace(userInfo.Email))
                {
                    usuario = await _dbContext.Usuarios
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Email == userInfo.Email, cancellationToken);
                }
                else
                {
                    usuario = null;
                }

                if (usuario == null)
                {
                    // Criar novo usuário
                    isNewUser = true;
                    usuario = new Usuario
                    {
                        Nome = userInfo.FullName ?? tokenResponse.Name ?? "Usuário ORCID",
                        Email = userInfo.Email ?? $"{orcidId}@orcid.temp",
                        Senha = Guid.NewGuid().ToString(), // Senha aleatória, usuário faz login via ORCID
                        Instituicao = string.Empty,
                        Pais = userInfo.Country ?? string.Empty,
                        DataCadastro = DateTime.UtcNow,
                        Ativo = true
                    };

                    usuario = await _usuarioRepository.CreateAsync(usuario, cancellationToken);
                    _logger.LogInformation("Novo usuário criado para ORCID: {Orcid}, UserId: {UserId}", orcidId, usuario.Id);
                }
                else
                {
                    _logger.LogInformation("Usuário existente encontrado para email: {Email}, vinculando ORCID", userInfo.Email);
                }

                // Criar novo perfil ORCID
                perfil = new PerfilORCID
                {
                    OrcidId = orcidId,
                    UsuarioId = usuario.Id,
                    NomeCompleto = userInfo.FullName ?? tokenResponse.Name ?? string.Empty,
                    Verificado = true,
                    Publicacoes = new List<string>(),
                    AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken,
                    TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    LastSyncAt = DateTime.UtcNow
                };

                perfil = await _perfilORCIDRepository.CreateAsync(perfil, cancellationToken);
                _logger.LogInformation("Perfil ORCID criado para ORCID: {Orcid}, PerfilId: {PerfilId}", orcidId, perfil.Id);
            }

            // 5. Atualizar tokens e informações do perfil
            perfil.AccessToken = tokenResponse.AccessToken;
            perfil.RefreshToken = tokenResponse.RefreshToken ?? perfil.RefreshToken;
            perfil.TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            perfil.LastSyncAt = DateTime.UtcNow;
            perfil.NomeCompleto = userInfo.FullName ?? tokenResponse.Name ?? perfil.NomeCompleto;
            perfil.Verificado = true;

            // Atualizar informações básicas do usuário se disponíveis
            if (!string.IsNullOrWhiteSpace(userInfo.FullName) && string.IsNullOrWhiteSpace(usuario.Nome))
            {
                usuario.Nome = userInfo.FullName;
            }

            if (!string.IsNullOrWhiteSpace(userInfo.Country) && string.IsNullOrWhiteSpace(usuario.Pais))
            {
                usuario.Pais = userInfo.Country;
            }

            if (isNewUser)
            {
                await _usuarioRepository.UpdateAsync(usuario, cancellationToken);
            }

            await _perfilORCIDRepository.UpdateAsync(perfil, cancellationToken);

            // 6. Retornar resposta de sucesso
            var loginResponse = new OrcidLoginResponseDTO
            {
                OrcidId = orcidId,
                Name = userInfo.FullName ?? tokenResponse.Name,
                Email = userInfo.Email,
                IsNewUser = isNewUser,
                UserId = usuario.Id,
                IsVerified = true,
                Message = isNewUser 
                    ? "Conta criada e vinculada ao ORCID com sucesso" 
                    : "Login realizado com sucesso"
            };

            _logger.LogInformation("Login ORCID bem-sucedido para ORCID: {Orcid}, UserId: {UserId}", orcidId, usuario.Id);
            return loginResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao processar callback OAuth");
            return null;
        }
    }

    public async Task<OrcidLoginResponseDTO?> LinkOrcidToUserAsync(long userId, string orcidId, string accessToken, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.FindByIdAsync(userId, cancellationToken);
        if (usuario == null)
        {
            _logger.LogWarning("Usuário não encontrado para vincular ORCID. UserId: {UserId}", userId);
            return null;
        }

        // Verificar se já existe perfil ORCID para este usuário
        var existingPerfil = await _dbContext.PerfisORCID
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UsuarioId == userId, cancellationToken);

        if (existingPerfil != null)
        {
            _logger.LogWarning("Usuário já possui perfil ORCID vinculado. UserId: {UserId}, PerfilId: {PerfilId}", userId, existingPerfil.Id);
            return null;
        }

        // Verificar se o ORCID ID já está vinculado a outro usuário
        var existingOrcid = await _dbContext.PerfisORCID
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrcidId == orcidId, cancellationToken);

        if (existingOrcid != null)
        {
            _logger.LogWarning("ORCID ID já está vinculado a outro usuário. OrcidId: {Orcid}, UserId: {UserId}", orcidId, existingOrcid.UsuarioId);
            return null;
        }

        try
        {
            // Buscar informações da pessoa no ORCID
            var person = await _orcidClient.GetPersonAsync(accessToken, orcidId, cancellationToken);
            if (person == null)
            {
                _logger.LogWarning("Falha ao buscar informações da pessoa para ORCID: {Orcid}", orcidId);
                return null;
            }

            var userInfo = OrcidUserInfoDTO.FromPersonDTO(orcidId, person);

            // Criar perfil ORCID
            var perfil = new PerfilORCID
            {
                OrcidId = orcidId,
                UsuarioId = usuario.Id,
                NomeCompleto = userInfo.FullName ?? string.Empty,
                Verificado = true,
                Publicacoes = new List<string>(),
                AccessToken = accessToken,
                RefreshToken = null, // Será atualizado quando houver refresh token
                TokenExpiresAt = DateTime.UtcNow.AddHours(1), // Valor padrão
                LastSyncAt = DateTime.UtcNow
            };

            perfil = await _perfilORCIDRepository.CreateAsync(perfil, cancellationToken);

            _logger.LogInformation("Perfil ORCID vinculado ao usuário. UserId: {UserId}, OrcidId: {Orcid}", userId, orcidId);

            return new OrcidLoginResponseDTO
            {
                OrcidId = orcidId,
                Name = userInfo.FullName,
                Email = userInfo.Email,
                IsNewUser = false,
                UserId = usuario.Id,
                IsVerified = true,
                Message = "ORCID vinculado com sucesso"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao vincular ORCID ao usuário. UserId: {UserId}, OrcidId: {Orcid}", userId, orcidId);
            return null;
        }
    }

    public async Task<bool> SyncUserProfileAsync(long userId, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.FindByIdAsync(userId, cancellationToken);
        if (usuario == null)
        {
            _logger.LogWarning("Usuário não encontrado para sincronização. UserId: {UserId}", userId);
            return false;
        }

        var perfil = await _dbContext.PerfisORCID
            .FirstOrDefaultAsync(p => p.UsuarioId == userId, cancellationToken);

        if (perfil == null)
        {
            _logger.LogWarning("Perfil ORCID não encontrado para sincronização. UserId: {UserId}", userId);
            return false;
        }

        // Verificar se o token expirou e tentar renovar
        if (perfil.TokenExpiresAt <= DateTime.UtcNow && !string.IsNullOrWhiteSpace(perfil.RefreshToken))
        {
            _logger.LogInformation("Token expirado, tentando renovar para ORCID: {Orcid}", perfil.OrcidId);
            var tokenResponse = await _orcidClient.RefreshTokenAsync(perfil.RefreshToken, cancellationToken);
            
            if (tokenResponse != null)
            {
                perfil.AccessToken = tokenResponse.AccessToken;
                perfil.RefreshToken = tokenResponse.RefreshToken ?? perfil.RefreshToken;
                perfil.TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            }
            else
            {
                _logger.LogWarning("Falha ao renovar token para ORCID: {Orcid}", perfil.OrcidId);
                return false;
            }
        }

        try
        {
            // Buscar informações atualizadas do ORCID
            var person = await _orcidClient.GetPersonAsync(perfil.AccessToken, perfil.OrcidId, cancellationToken);
            if (person == null)
            {
                _logger.LogWarning("Falha ao buscar informações atualizadas para ORCID: {Orcid}", perfil.OrcidId);
                return false;
            }

            var userInfo = OrcidUserInfoDTO.FromPersonDTO(perfil.OrcidId, person);

            // Atualizar perfil
            perfil.NomeCompleto = userInfo.FullName ?? perfil.NomeCompleto;
            perfil.LastSyncAt = DateTime.UtcNow;

            // Atualizar informações do usuário se necessário
            if (!string.IsNullOrWhiteSpace(userInfo.FullName) && usuario.Nome != userInfo.FullName)
            {
                usuario.Nome = userInfo.FullName;
            }

            if (!string.IsNullOrWhiteSpace(userInfo.Country) && usuario.Pais != userInfo.Country)
            {
                usuario.Pais = userInfo.Country;
            }

            await _perfilORCIDRepository.UpdateAsync(perfil, cancellationToken);
            await _usuarioRepository.UpdateAsync(usuario, cancellationToken);

            _logger.LogInformation("Perfil sincronizado com sucesso. UserId: {UserId}, OrcidId: {Orcid}", userId, perfil.OrcidId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar perfil. UserId: {UserId}", userId);
            return false;
        }
    }

    public async Task<OrcidUserInfoDTO?> GetUserOrcidAsync(long userId, CancellationToken cancellationToken = default)
    {
        var perfil = await _dbContext.PerfisORCID
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UsuarioId == userId, cancellationToken);

        if (perfil == null)
        {
            _logger.LogWarning("Perfil ORCID não encontrado para usuário. UserId: {UserId}", userId);
            return null;
        }

        // Buscar informações atualizadas do ORCID
        var person = await _orcidClient.GetPersonAsync(perfil.AccessToken, perfil.OrcidId, cancellationToken);
        if (person == null)
        {
            _logger.LogWarning("Falha ao buscar informações do ORCID para usuário. UserId: {UserId}, OrcidId: {Orcid}", userId, perfil.OrcidId);
            return null;
        }

        return OrcidUserInfoDTO.FromPersonDTO(perfil.OrcidId, person);
    }

    public async Task<bool> RevokeUserTokenAsync(long userId, CancellationToken cancellationToken = default)
    {
        var perfil = await _dbContext.PerfisORCID
            .FirstOrDefaultAsync(p => p.UsuarioId == userId, cancellationToken);

        if (perfil == null)
        {
            _logger.LogWarning("Perfil ORCID não encontrado para revogação. UserId: {UserId}", userId);
            return false;
        }

        try
        {
            // Revogar access token
            var accessTokenRevoked = await _orcidClient.RevokeTokenAsync(perfil.AccessToken, "access_token", cancellationToken);
            
            // Revogar refresh token se disponível
            if (!string.IsNullOrWhiteSpace(perfil.RefreshToken))
            {
                await _orcidClient.RevokeTokenAsync(perfil.RefreshToken, "refresh_token", cancellationToken);
            }

            // Limpar tokens do perfil
            perfil.AccessToken = string.Empty;
            perfil.RefreshToken = null;
            perfil.TokenExpiresAt = DateTime.UtcNow;
            perfil.Verificado = false;

            await _perfilORCIDRepository.UpdateAsync(perfil, cancellationToken);

            _logger.LogInformation("Token revogado com sucesso para usuário. UserId: {UserId}, OrcidId: {Orcid}", userId, perfil.OrcidId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao revogar token para usuário. UserId: {UserId}", userId);
            return false;
        }
    }
}

