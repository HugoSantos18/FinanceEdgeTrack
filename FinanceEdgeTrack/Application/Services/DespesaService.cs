using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Error;
using MapsterMapper;

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

        public async Task<DespesaDTO> ObterDespesaPorIdAsync(Guid id)
        {
            var despesa = await _uof.DespesaRepository.Get(d => d.DespesaId == id);

            return _mapper.Map<DespesaDTO>(despesa);
        }

        public async Task<IReadOnlyList<DespesaDTO>> ListarDespesasAsync()
        {
            var despesas = await _uof.DespesaRepository.GetAll();

            return _mapper.Map<IReadOnlyList<DespesaDTO>>(despesas);
        }

        public async Task<DespesaDTO> CreateDespesaAsync(CreateDespesaDTO despesaDto)
        {
            var despesa = _mapper.Map<Despesa>(despesaDto);

            await _carteiraService.DescontarSaldoAsync(_currentUser.UserId, despesa.Valor);
            await _uof.DespesaRepository.Create(despesa);

            return _mapper.Map<DespesaDTO>(despesa);
        }

        public async Task AtualizarDespesaAsync(Guid id, UpdateDespesaDTO despesaDto)
        {
            var despesa = await _uof.DespesaRepository.Get(d => d.DespesaId == id);

            if (despesa is null)
                throw new KeyNotFoundException(ResultMessages.NotFoundDespesa);

            despesa.Titulo = despesaDto.Titulo;
            despesa.Descricao = despesaDto.Descricao;
            despesa.Data = despesaDto.Data;
            despesa.Valor = despesaDto.Valor;

            await _uof.DespesaRepository.Update(despesa)!;
        }


        public async Task RemoverDespesaAsync(Guid id)
        {
            var despesaRemovida = await _uof.DespesaRepository.Get(d => d.DespesaId == id);

            if (despesaRemovida is null)
                throw new KeyNotFoundException(ResultMessages.NotFoundDespesa);

            await _carteiraService.AdicionarSaldoAsync(_currentUser.UserId, despesaRemovida.Valor);
            await _uof.DespesaRepository.Delete(despesaRemovida)!;
        }
    }
}
