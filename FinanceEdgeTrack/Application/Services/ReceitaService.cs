using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Repositories;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using MapsterMapper;

namespace FinanceEdgeTrack.Application.Services;

public class ReceitaService : IReceitaService
{
    private const string NotFoundMessage = "Receita não encontrada";
    private readonly IUnitOfWork _uof;
    private readonly ICarteiraService _carteiraService;
    private readonly IMapper _mapper;

    public ReceitaService(IUnitOfWork uof, IMapper mapper, ICarteiraService carteira)
    {
        this._mapper = mapper;
        this._uof = uof;
        this._carteiraService = carteira;
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

        await _uof.ReceitaRepository.Create(receita);
        await _carteiraService.AdicionarSaldo(receita.Valor); // VERIFICAR LÓGICA DA CARTEIRA. (SÓ ADD OU TER UserID)

        return _mapper.Map<ReceitaDTO>(receita); 
    }

    public async Task AtualizarReceitaAsync(Guid id, UpdateReceitaDTO receitaDto)
    {
        var receita = await _uof.ReceitaRepository.Get(r => r.ReceitaId == id);
        
        if (receita is null)
            throw new KeyNotFoundException(NotFoundMessage);
        
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
            throw new KeyNotFoundException(NotFoundMessage);
        
        await _uof.ReceitaRepository.Delete(receitaRemovida)!;
        await _carteiraService.DescontarSaldo(receitaRemovida.Valor);
    }
}
