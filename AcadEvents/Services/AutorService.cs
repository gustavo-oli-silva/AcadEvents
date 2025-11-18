using AcadEvents.Dtos;
using AcadEvents.Models;
using AcadEvents.Repositories;

namespace AcadEvents.Services;

public class AutorService
{
    private readonly AutorRepository _autorRepository;
    private readonly PerfilORCIDRepository _perfilORCIDRepository;

    public AutorService(
        AutorRepository autorRepository,
        PerfilORCIDRepository perfilORCIDRepository)
    {
        _autorRepository = autorRepository;
        _perfilORCIDRepository = perfilORCIDRepository;
    }

    public async Task<List<Autor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _autorRepository.FindAllAsync(cancellationToken);
    }

    public async Task<Autor?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _autorRepository.FindByIdAsync(id, cancellationToken);
    }

    public async Task<Autor> CreateAsync(AutorRequestDTO request, CancellationToken cancellationToken = default)
    {
        // Verificar se o PerfilORCID existe (se fornecido)
        if (request.PerfilORCIDId.HasValue)
        {
            var perfil = await _perfilORCIDRepository.FindByIdAsync(request.PerfilORCIDId.Value, cancellationToken);
            if (perfil == null)
                throw new ArgumentException($"Perfil ORCID com Id {request.PerfilORCIDId.Value} não encontrado.");
        }

        var autor = new Autor
        {
            Nome = request.Nome,
            Email = request.Email,
            Senha = request.Senha,
            Instituicao = request.Instituicao,
            Pais = request.Pais,
            DataCadastro = DateTime.UtcNow,
            Ativo = true,
            Biografia = request.Biografia,
            AreaAtuacao = request.AreaAtuacao,
            Lattes = request.Lattes,
            PerfilORCIDId = request.PerfilORCIDId
        };

        return await _autorRepository.CreateAsync(autor, cancellationToken);
    }

    public async Task<Autor?> UpdateAsync(long id, AutorRequestDTO request, CancellationToken cancellationToken = default)
    {
        var autor = await _autorRepository.FindByIdAsync(id, cancellationToken);
        if (autor == null)
            return null;

        // Verificar se o PerfilORCID existe (se fornecido)
        if (request.PerfilORCIDId.HasValue)
        {
            var perfil = await _perfilORCIDRepository.FindByIdAsync(request.PerfilORCIDId.Value, cancellationToken);
            if (perfil == null)
                throw new ArgumentException($"Perfil ORCID com Id {request.PerfilORCIDId.Value} não encontrado.");
        }

        autor.Nome = request.Nome;
        autor.Email = request.Email;
        autor.Senha = request.Senha;
        autor.Instituicao = request.Instituicao;
        autor.Pais = request.Pais;
        autor.Biografia = request.Biografia;
        autor.AreaAtuacao = request.AreaAtuacao;
        autor.Lattes = request.Lattes;
        autor.PerfilORCIDId = request.PerfilORCIDId;

        return await _autorRepository.UpdateAsync(autor, cancellationToken);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _autorRepository.DeleteAsync(id, cancellationToken);
    }
}

