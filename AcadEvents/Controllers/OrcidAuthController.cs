using Microsoft.AspNetCore.Mvc;
using AcadEvents.Dtos.Orcid;
using AcadEvents.Services;

namespace AcadEvents.Controllers;

[Route("api/auth/orcid")]
[ApiController]
public class OrcidAuthController : ControllerBase
{
    private readonly IOrcidService _orcidService;

    public OrcidAuthController(IOrcidService orcidService)
    {
        _orcidService = orcidService;
    }

    /// <summary>
    /// Inicia o fluxo OAuth do ORCID
    /// Retorna a URL de autorização para redirecionar o usuário
    /// </summary>
    [HttpGet("login")]
    public async Task<ActionResult<object>> Login([FromQuery] string? state = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var authUrl = await _orcidService.GetAuthorizationUrlAsync(state, cancellationToken);
            
            return Ok(new
            {
                AuthorizationUrl = authUrl,
                Message = "Redirecione o usuário para esta URL para iniciar a autenticação ORCID"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Erro ao gerar URL de autorização", Message = ex.Message });
        }
    }

    /// <summary>
    /// Callback do ORCID após autorização do usuário
    /// Este endpoint recebe o código de autorização e processa o login
    /// </summary>
    [HttpGet("callback")]
    public async Task<ActionResult<OrcidLoginResponseDTO>> Callback(
        [FromQuery] string? code = null,
        [FromQuery] string? state = null,
        [FromQuery] string? error = null,
        [FromQuery] string? error_description = null,
        CancellationToken cancellationToken = default)
    {
        // Verificar se houve erro na autorização
        if (!string.IsNullOrWhiteSpace(error))
        {
            return BadRequest(new
            {
                Error = error,
                ErrorDescription = error_description,
                Message = "Erro na autorização ORCID"
            });
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return BadRequest(new { Error = "Código de autorização não fornecido" });
        }

        try
        {
            var loginResponse = await _orcidService.HandleCallbackAsync(code, state, cancellationToken);
            
            if (loginResponse == null)
            {
                return StatusCode(500, new { Error = "Falha ao processar autenticação ORCID" });
            }

            return Ok(loginResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Erro ao processar callback OAuth", Message = ex.Message });
        }
    }

    /// <summary>
    /// Vincula um perfil ORCID a um usuário existente no sistema
    /// </summary>
    [HttpPost("link/{userId}")]
    public async Task<ActionResult<OrcidLoginResponseDTO>> LinkOrcidToUser(
        long userId,
        [FromBody] LinkOrcidRequestDTO request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.OrcidId) || string.IsNullOrWhiteSpace(request.AccessToken))
        {
            return BadRequest(new { Error = "ORCID ID e Access Token são obrigatórios" });
        }

        try
        {
            var linkResponse = await _orcidService.LinkOrcidToUserAsync(
                userId, 
                request.OrcidId, 
                request.AccessToken, 
                cancellationToken);

            if (linkResponse == null)
            {
                return BadRequest(new { Error = "Falha ao vincular ORCID ao usuário. Verifique se o usuário existe e se o ORCID não está vinculado a outro usuário." });
            }

            return Ok(linkResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Erro ao vincular ORCID ao usuário", Message = ex.Message });
        }
    }

    /// <summary>
    /// Sincroniza os dados do perfil ORCID com o perfil local do usuário
    /// </summary>
    [HttpPost("sync/{userId}")]
    public async Task<ActionResult<object>> SyncProfile(long userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _orcidService.SyncUserProfileAsync(userId, cancellationToken);
            
            if (!success)
            {
                return NotFound(new { Error = "Usuário ou perfil ORCID não encontrado, ou falha na sincronização" });
            }

            return Ok(new { Message = "Perfil sincronizado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Erro ao sincronizar perfil", Message = ex.Message });
        }
    }

    /// <summary>
    /// Retorna as informações ORCID de um usuário
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<OrcidUserInfoDTO>> GetUserOrcid(long userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var orcidInfo = await _orcidService.GetUserOrcidAsync(userId, cancellationToken);
            
            if (orcidInfo == null)
            {
                return NotFound(new { Error = "Perfil ORCID não encontrado para este usuário" });
            }

            return Ok(orcidInfo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Erro ao buscar informações ORCID do usuário", Message = ex.Message });
        }
    }

    /// <summary>
    /// Revoga o token ORCID de um usuário (logout do ORCID)
    /// </summary>
    [HttpPost("revoke/{userId}")]
    public async Task<ActionResult<object>> RevokeToken(long userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _orcidService.RevokeUserTokenAsync(userId, cancellationToken);
            
            if (!success)
            {
                return NotFound(new { Error = "Perfil ORCID não encontrado para este usuário" });
            }

            return Ok(new { Message = "Token revogado com sucesso" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Erro ao revogar token", Message = ex.Message });
        }
    }
}

/// <summary>
/// DTO para requisição de vinculação de ORCID a usuário
/// </summary>
public record LinkOrcidRequestDTO
{
    public string OrcidId { get; init; } = string.Empty;
    public string AccessToken { get; init; } = string.Empty;
}

