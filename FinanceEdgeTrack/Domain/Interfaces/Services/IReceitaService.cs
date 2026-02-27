using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface IReceitaService
{
    Task<ReceitaDTO> CreateReceitaAsync(CreateReceitaDTO receitaDto);
    Task AtualizarReceitaAsync(Guid id, UpdateReceitaDTO receitaDto);
    Task RemoverReceitaAsync(Guid id);
    Task<ReceitaDTO> ObterReceitaPorIdAsync(Guid id);
    Task<IReadOnlyList<ReceitaDTO>> ListarReceitasAsync();

    // métodos com filtros e paginação posteriormente.
}
