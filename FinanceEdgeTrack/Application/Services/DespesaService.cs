using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace FinanceEdgeTrack.Application.Services
{
    public class DespesaService : IDespesaService
    {
        private readonly IUnitOfWork _uof;
        private readonly ICarteiraService _carteiraService;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public DespesaService(IUnitOfWork uof, IMapper mapper, ICarteiraService carteira, ICurrentUserService currentUser)
        {
            this._mapper = mapper;
            this._uof = uof;
            this._carteiraService = carteira;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<DespesaDTO>> ObterDespesaPorIdAsync(Guid id)
        {
            var despesa = await _uof.DespesaRepository.GetAsync(d => d.DespesaId == id);

            if (despesa is null)
                return ApiResponse<DespesaDTO>.Fail(ResultMessages.NotFoundDespesa);

            var despesaDto = _mapper.Map<DespesaDTO>(despesa);

            return ApiResponse<DespesaDTO>.Ok(despesaDto);
        }

        public async Task<ApiResponse<IReadOnlyList<DespesaDTO>>> ListarDespesasAsync()
        {
            var despesas = await _uof.DespesaRepository.GetAllAsync();
            
            if (despesas is null)
                return ApiResponse<IReadOnlyList<DespesaDTO>>.Fail(ResultMessages.NotFoundDespesa);

            var despesasDto = _mapper.Map<IReadOnlyList<DespesaDTO>>(despesas);
       
           return ApiResponse<IReadOnlyList<DespesaDTO>>.Ok(despesasDto);
        }

        public async Task<ApiResponse<DespesaDTO>> CreateDespesaAsync(CreateDespesaDTO despesaDto)
        {
            var despesa = _mapper.Map<Despesa>(despesaDto);

            await _carteiraService.DescontarSaldoAsync(_currentUser.UserId, despesa.Valor);
            await _uof.DespesaRepository.CreateAsync(despesa);

            return ApiResponse<DespesaDTO>.Ok(_mapper.Map<DespesaDTO>(despesa));
        }

        public async Task<ApiResponse<DespesaDTO>> AtualizarDespesaAsync(Guid id, UpdateDespesaDTO despesaDto)
        {
            var despesa = await _uof.DespesaRepository.GetAsync(d => d.DespesaId == id);

            if (despesa is null)
                return ApiResponse<DespesaDTO>.Fail(ResultMessages.NotFoundDespesa);

            despesa.Titulo = despesaDto.Titulo!;
            despesa.Descricao = despesaDto.Descricao;
            despesa.Data = despesaDto.Data;
            despesa.Valor = despesaDto.Valor;
            despesaDto.UpdatedAt = DateTime.UtcNow.ToShortDateString();

            await _uof.DespesaRepository.UpdateAsync(despesa)!;

            return ApiResponse<DespesaDTO>.Ok(_mapper.Map<DespesaDTO>(despesa));
        }


        public async Task<ApiResponse<DespesaDTO>> RemoverDespesaAsync(Guid id)
        {
            var despesaRemovida = await _uof.DespesaRepository.GetAsync(d => d.DespesaId == id);

            if (despesaRemovida is null)
                return ApiResponse<DespesaDTO>.Fail(ResultMessages.NotFoundDespesa);

            await _carteiraService.AdicionarSaldoAsync(_currentUser.UserId, despesaRemovida.Valor);
            await _uof.DespesaRepository.DeleteAsync(despesaRemovida)!;

            return ApiResponse<DespesaDTO>.Ok(_mapper.Map<DespesaDTO>(despesaRemovida), "Despesa removida com sucesso");
        }
    }
}
