using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using MapsterMapper;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Error;
using Mapster;

namespace FinanceEdgeTrack.Application.Services;

public class MetaService : IMetaService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uof;
    private readonly ICarteiraService _carteira;
    private readonly ICurrentUserService _currentUser;

    public MetaService(IMapper mapper, IUnitOfWork uof, ICarteiraService carteira, ICurrentUserService currentUser)
    {
        this._mapper = mapper;
        this._uof = uof;
        _carteira = carteira;
        _currentUser = currentUser;
    }

    public async Task<MetaDTO> GetMetaPorIdAsync(Guid metaId)
    {
        var meta = await _uof.MetaRepository.Get(m => m.MetaId == metaId);

        if (meta is null)
            throw new KeyNotFoundException();

        return _mapper.Map<MetaDTO>(meta);
    }

    public async Task<AporteMetasDTO> GetAportePorIdAsync(Guid aporteMetaId)
    {
        var meta = await _uof.MetaRepository.Get(m => m.Aportes.Any(a => a.Id == aporteMetaId)) ?? throw new KeyNotFoundException(ResultMessages.NotFoundMeta);

        var aporte = meta.Aportes.First(a => a.Id == aporteMetaId);

        return _mapper.Map<AporteMetasDTO>(aporte);
    }

    public async Task<IReadOnlyList<AporteMetasDTO>> GetAllAportesDaMetaPorIdAsync(Guid metaId)
    {
        var meta = await _uof.MetaRepository.Get(m => m.MetaId == metaId) ?? throw new KeyNotFoundException(ResultMessages.NotFoundMeta); ;

        var aportes = meta.Aportes;

        return _mapper.Map<IReadOnlyList<AporteMetasDTO>>(aportes);
    }

    public async Task<IReadOnlyList<MetaDTO>> GetAllMetasAsync()
    {
        var metas = await _uof.MetaRepository.GetAll() ?? throw new KeyNotFoundException(ResultMessages.NotFoundMeta);

        return _mapper.Map<IReadOnlyList<MetaDTO>>(metas);
    }


    public async Task AtualizarMetaAsync(Guid metaId, UpdateMetaDTO metaDto)
    {
        var meta = await _uof.MetaRepository.Get(m => m.MetaId == metaId) ?? throw new KeyNotFoundException(ResultMessages.NotFoundMeta);

        meta.Titulo = metaDto.Titulo;
        meta.ValorAlvo = metaDto.ValorAlvo;
        meta.AlterarDataAlvo(metaDto.DataAlvo);
        meta.AlterarStatus(metaDto.Status);

        await _uof.MetaRepository.Update(meta);
    }

    public async Task<MetaDTO> CriarMetaAsync(CreateMetaDTO metaDto)
    {
        var meta = _mapper.Map<Meta>(metaDto) ?? throw new InvalidOperationException(ResultMessages.ValidMeta);

        await _uof.MetaRepository.Create(meta);

        return _mapper.Map<MetaDTO>(meta);
    }

    public async Task FinalizarMeta(Guid metaId)
    {
        var meta = await _uof.MetaRepository.Get(m => m.MetaId == metaId) ?? throw new KeyNotFoundException(ResultMessages.NotFoundMeta);

        meta.FinalizarMeta();
        await _uof.CommitAsync();
    }


    public async Task<AporteMetasDTO> RegistrarAporteAsync(Guid metaId, CreateAporteMetaDTO aporteMetaDto)
    {
        var meta = await _uof.MetaRepository.Get(m => m.MetaId == metaId) ?? throw new KeyNotFoundException(ResultMessages.NotFoundMeta);

        var novoAporte = _mapper.Map<AporteMetas>(aporteMetaDto) ?? throw new InvalidOperationException(ResultMessages.ErrorCreation);

        await _carteira.DescontarSaldoAsync(_currentUser.UserId, novoAporte.Valor);
        
        meta.RegistrarAporte(novoAporte);
        await _uof.CommitAsync();

        return _mapper.Map<AporteMetasDTO>(novoAporte);
    }

    public async Task RemoverAporteAsync(Guid aporteMetaId)
    {
        var meta = await _uof.MetaRepository.Get(m => m.Aportes.Any(a => a.Id == aporteMetaId)) ?? throw new KeyNotFoundException(ResultMessages.NotFoundAporte);

        var aporteRemovido = meta.Aportes.First(a => a.Id == aporteMetaId) ?? throw new KeyNotFoundException(ResultMessages.NotFoundAporte);

        await _carteira.AdicionarSaldoAsync(_currentUser.UserId, aporteRemovido.Valor);
        meta.RemoverAporte(aporteRemovido);
        
        await _uof.CommitAsync();
    }

    public async Task RemoverMetaAsync(Guid metaId)
    {
        var meta = await _uof.MetaRepository.Get(m => m.MetaId == metaId) ?? throw new KeyNotFoundException(ResultMessages.NotFoundMeta);

        await _uof.MetaRepository.Delete(meta);
    }

    public async Task<decimal> ValorTotalEmAportes(Guid metaId)
    {
        var meta = await _uof.MetaRepository.Get(m => m.MetaId == metaId) ?? throw new KeyNotFoundException(ResultMessages.NotFoundMeta);

        return meta.ValorTotalAportes();
    }
}
