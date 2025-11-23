using AcadEvents.Dtos;
using AcadEvents.Models;
using AcadEvents.Repositories;

namespace AcadEvents.Services;

public class SubmissaoService
{
    private readonly SubmissaoRepository _submissaoRepository;
    private readonly AutorRepository _autorRepository;
    private readonly EventoRepository _eventoRepository;
    private readonly TrilhaTematicaRepository _trilhaTematicaRepository;

    public SubmissaoService(
        SubmissaoRepository submissaoRepository,
        AutorRepository autorRepository,
        EventoRepository eventoRepository,
        TrilhaTematicaRepository trilhaTematicaRepository)
    {
        _submissaoRepository = submissaoRepository;
        _autorRepository = autorRepository;
        _eventoRepository = eventoRepository;
        _trilhaTematicaRepository = trilhaTematicaRepository;
    }

    public Task<List<Submissao>> GetAllAsync(CancellationToken cancellationToken = default)
        => _submissaoRepository.FindAllAsync(cancellationToken);

    public Task<Submissao?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => _submissaoRepository.FindByIdAsync(id, cancellationToken);

    public async Task<Submissao> CreateAsync(SubmissaoRequestDTO request, long autorId, CancellationToken cancellationToken = default)
    {
        await ValidateReferencesAsync(request, autorId, cancellationToken);

        var submissao = MapToEntity(new Submissao(), request, autorId);
        return await _submissaoRepository.CreateAsync(submissao, cancellationToken);
    }

    public async Task<Submissao?> UpdateAsync(long id, SubmissaoRequestDTO request, CancellationToken cancellationToken = default)
    {
        var submissao = await _submissaoRepository.FindByIdAsync(id, cancellationToken);
        if (submissao is null)
        {
            return null;
        }

        // No update, mantemos o autor original (ou pode extrair do token se necessário)
        await ValidateReferencesAsync(request, submissao.AutorId, cancellationToken);

        MapToEntity(submissao, request, submissao.AutorId);
        await _submissaoRepository.UpdateAsync(submissao, cancellationToken);
        return submissao;
    }

    public Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
        => _submissaoRepository.DeleteAsync(id, cancellationToken);

    private Submissao MapToEntity(Submissao entity, SubmissaoRequestDTO request, long autorId)
    {
        entity.Titulo = request.Titulo;
        entity.Resumo = request.Resumo;
        entity.PalavrasChave = request.PalavrasChave ?? new List<string>();
        entity.DataSubmissao = request.DataSubmissao;
        entity.DataUltimaModificacao = request.DataUltimaModificacao;
        entity.Versao = request.Versao;
        entity.Status = request.Status;
        entity.Formato = request.Formato;
        entity.AutorId = autorId;
        entity.EventoId = request.EventoId;
        entity.TrilhaTematicaId = request.TrilhaTematicaId;
        // SessaoId e DOIId não são mais definidos na criação
        // entity.SessaoId = null;
        // entity.DOIId = null;
        return entity;
    }

    private async Task ValidateReferencesAsync(SubmissaoRequestDTO request, long autorId, CancellationToken cancellationToken)
    {
        if (!await _autorRepository.ExistsAsync(autorId, cancellationToken))
        {
            throw new ArgumentException($"Autor {autorId} não existe.");
        }

        if (!await _eventoRepository.ExistsAsync(request.EventoId, cancellationToken))
        {
            throw new ArgumentException($"Evento {request.EventoId} não existe.");
        }

        if (!await _trilhaTematicaRepository.ExistsAsync(request.TrilhaTematicaId, cancellationToken))
        {
            throw new ArgumentException($"Trilha temática {request.TrilhaTematicaId} não existe.");
        }

        // Validações de SessaoId e DOIId removidas - não são mais necessárias na criação
    }
}


