using Microsoft.AspNetCore.Mvc;
using AcadEvents.Services;

namespace AcadEvents.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReferenciaController : ControllerBase
{
    private readonly ReferenciaService _referenciaService;

    public ReferenciaController(ReferenciaService referenciaService)
    {
        _referenciaService = referenciaService;
    }

    [HttpPost("doi/{doi}")]
    public async Task<ActionResult> CreateFromDoi(
        string doi,
        [FromQuery] long? submissaoId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var referencia = await _referenciaService.CreateFromDoiAsync(doi, submissaoId, cancellationToken);
            return CreatedAtAction(
                nameof(GetById),
                new { id = referencia.Id },
                new
                {
                    id = referencia.Id,
                    doi = doi,
                    titulo = referencia.Titulo,
                    autores = referencia.Autores,
                    ano = referencia.Ano,
                    publicacao = referencia.Publicacao
                });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao processar a requisição.", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(long id, CancellationToken cancellationToken = default)
    {
        // TODO: Implementar busca por ID quando necessário
        return Ok(new { message = "Endpoint de busca por ID ainda não implementado." });
    }
}

