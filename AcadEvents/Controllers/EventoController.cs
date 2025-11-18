using Microsoft.AspNetCore.Mvc;
using AcadEvents.Dtos;
using AcadEvents.Services;

namespace AcadEvents.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly EventoService _eventoService;

        public EventoController(EventoService eventoService)
        {
            _eventoService = eventoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EventoResponseDTO>>> GetAll(CancellationToken cancellationToken = default)
        {
            var eventos = await _eventoService.GetAllAsync(cancellationToken);
            var eventosDTO = eventos.Select(e => EventoResponseDTO.ValueOf(e)).ToList();
            return Ok(eventosDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventoResponseDTO>> GetById(long id, CancellationToken cancellationToken = default)
        {
            var evento = await _eventoService.GetByIdAsync(id, cancellationToken);
            if (evento == null)
                return NotFound($"Evento com Id {id} não encontrado.");

            return Ok(EventoResponseDTO.ValueOf(evento));
        }

        [HttpPost("organizador/{organizadorId}")]
        public async Task<ActionResult<EventoResponseDTO>> Create(
            long organizadorId,
            [FromBody] EventoRequestDTO request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var evento = await _eventoService.CreateAsync(organizadorId, request, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = evento.Id }, EventoResponseDTO.ValueOf(evento));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            long id,
            [FromBody] EventoRequestDTO request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var evento = await _eventoService.UpdateAsync(id, request, cancellationToken);
                if (evento == null)
                    return NotFound($"Evento com Id {id} não encontrado.");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken = default)
        {
            var deletado = await _eventoService.DeleteAsync(id, cancellationToken);
            if (!deletado)
                return NotFound($"Evento com Id {id} não encontrado.");

            return NoContent();
        }

        [HttpPost("{eventoId}/organizadores/{organizadorId}")]
        public async Task<ActionResult<EventoResponseDTO>> AddOrganizador(
            long eventoId,
            long organizadorId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var evento = await _eventoService.AddOrganizadorAsync(eventoId, organizadorId, cancellationToken);
                return Ok(EventoResponseDTO.ValueOf(evento));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{eventoId}/organizadores/{organizadorId}")]
        public async Task<ActionResult<EventoResponseDTO>> RemoveOrganizador(
            long eventoId,
            long organizadorId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var evento = await _eventoService.RemoveOrganizadorAsync(eventoId, organizadorId, cancellationToken);
                return Ok(EventoResponseDTO.ValueOf(evento));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
