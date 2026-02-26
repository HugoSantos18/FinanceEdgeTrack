using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Interfaces.Services;

namespace FinanceEdgeTrack.Application.Services
{
    public class DespesaService : IDespesaService
    {
        public Task AtualizarDespesaAsync(Guid id, UpdateDespesaDTO despesaDto)
        {
            throw new NotImplementedException();
        }

        public Task<DespesaDTO> CreateDespesaAsync(DespesaDTO despesaDto)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<DespesaDTO>> ListarDespesasAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DespesaDTO> ObterDespesaPorIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RemoverDespesaAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
