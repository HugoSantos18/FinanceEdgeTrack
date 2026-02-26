using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Error;
using MapsterMapper;

namespace FinanceEdgeTrack.Application.Services;

public class ReceitaService : IReceitaService
{
    
    private readonly IUnitOfWork _uof;
    private readonly ICarteiraService _carteiraService;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public ReceitaService(IUnitOfWork uof, IMapper mapper, ICarteiraService carteira, ICurrentUserService currentUser)
    {
        this._mapper = mapper;
        this._uof = uof;
        this._carteiraService = carteira;
        _currentUser = currentUser;
    }

    public async Task<ReceitaDTO> ObterReceitaPorIdAsync(Guid id)
    {
        var receita = await _uof.ReceitaRepository.Get(r => r.ReceitaId == id);

        return _mapper.Map<ReceitaDTO>(receita);
    }

    public async Task<IReadOnlyList<ReceitaDTO>> ListarReceitasAsync()
    {
        var receitas = await _uof.ReceitaRepository.GetAll();

        return _mapper.Map<IReadOnlyList<ReceitaDTO>>(receitas);
    }

    public async Task<ReceitaDTO> CreateReceitaAsync(CreateReceitaDTO receitaDto)
    {
        var receita = _mapper.Map<Receita>(receitaDto);

        await _carteiraService.AdicionarSaldoAsync(_currentUser.UserId, receita.Valor);
        await _uof.ReceitaRepository.Create(receita);

        return _mapper.Map<ReceitaDTO>(receita); 
    }

    public async Task AtualizarReceitaAsync(Guid id, UpdateReceitaDTO receitaDto)
    {
        var receita = await _uof.ReceitaRepository.Get(r => r.ReceitaId == id);
        
        if (receita is null)
            throw new KeyNotFoundException(ErrorMessages.NotFoundReceiveMessage);
        
        receita.Titulo = receitaDto.Titulo;
        receita.Descricao = receitaDto.Descricao;
        receita.Data = receitaDto.Data;
        receita.Valor = receitaDto.Valor;

        await _uof.ReceitaRepository.Update(receita)!;
    }


    public async Task RemoverReceitaAsync(Guid id)
    {
        var receitaRemovida = await _uof.ReceitaRepository.Get(r => r.ReceitaId == id);
        
        if (receitaRemovida is null)
            throw new KeyNotFoundException(ErrorMessages.NotFoundReceiveMessage);
     
        await _carteiraService.DescontarSaldoAsync(_currentUser.UserId, receitaRemovida.Valor);
        await _uof.ReceitaRepository.Delete(receitaRemovida)!;
    }
}
