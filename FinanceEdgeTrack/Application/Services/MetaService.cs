using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using MapsterMapper;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Error;
using Mapster;
using FinanceEdgeTrack.Application.Common;

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

    public async Task<ApiResponse<MetaDTO>> GetMetaPorIdAsync(Guid metaId)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.MetaId == metaId);

        if (meta is null)
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

    public async Task<ApiResponse<AporteMetasDTO>> GetAportePorIdAsync(Guid aporteMetaId)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.Aportes.Any(a => a.Id == aporteMetaId));

        if (meta is null)
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundMeta);

        var aporte = meta.Aportes.First(a => a.Id == aporteMetaId);

        return ApiResponse<AporteMetasDTO>.Ok(_mapper.Map<AporteMetasDTO>(aporte));
    }

    public async Task<ApiResponse<IReadOnlyList<AporteMetasDTO>>> GetAllAportesDaMetaPorIdAsync(Guid metaId)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.MetaId == metaId);

        if (meta is null)
            return ApiResponse<IReadOnlyList<AporteMetasDTO>>.Fail(ResultMessages.NotFoundMeta);

        var aportes = meta.Aportes;

        return ApiResponse<IReadOnlyList<AporteMetasDTO>>.Ok(_mapper.Map<IReadOnlyList<AporteMetasDTO>>(aportes));
    }

    public async Task<ApiResponse<IReadOnlyList<MetaDTO>>> GetAllMetasAsync()
    {
        var metas = await _uof.MetaRepository.GetAllAsync();

        if (metas is null)
            return ApiResponse<IReadOnlyList<MetaDTO>>.Fail(ResultMessages.NotFoundMeta);

        return ApiResponse<IReadOnlyList<MetaDTO>>.Ok(_mapper.Map<IReadOnlyList<MetaDTO>>(metas));
    }


    public async Task<ApiResponse<MetaDTO>> AtualizarMetaAsync(Guid metaId, UpdateMetaDTO metaDto)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.MetaId == metaId);

        if (meta is null)
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);

        meta.Titulo = metaDto.Titulo;
        meta.ValorAlvo = metaDto.ValorAlvo;
        meta.AlterarDataAlvo(metaDto.DataAlvo);
        meta.AlterarStatus(metaDto.Status);
        await _uof.MetaRepository.UpdateAsync(meta);

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta), $"Meta {meta.Titulo} atualizada com sucesso");
    }

    public async Task<ApiResponse<MetaDTO>> CriarMetaAsync(CreateMetaDTO metaDto)
    {
        var meta = _mapper.Map<Meta>(metaDto) ?? throw new InvalidOperationException(ResultMessages.ValidMeta);

        await _uof.MetaRepository.CreateAsync(meta);

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

    public async Task<ApiResponse<MetaDTO>> FinalizarMeta(Guid metaId)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.MetaId == metaId);

        if (meta is null)
            return ApiResponse<MetaDTO>.Fail(ResultMessages.ValidMeta);

        meta.FinalizarMeta();
        await _uof.CommitAsync();

        int daysCompleted = meta.DataInicio.Day - DateTime.UtcNow.Day;
        int daysRest = DateTime.UtcNow.Day - meta.DataAlvo.Day;

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta),
            $"Parábens você finalizou sua meta em {daysCompleted} dias " +
            $"e com a data que foi alvejada teve a diferença de {daysRest} dias");
    }


    public async Task<ApiResponse<AporteMetasDTO>> RegistrarAporteAsync(Guid metaId, CreateAporteMetaDTO aporteMetaDto)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.MetaId == metaId);

        if (meta is null)
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundMeta);

        var novoAporte = _mapper.Map<AporteMetas>(aporteMetaDto);

        if (novoAporte is null)
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.ErrorCreation);

        await _carteira.DescontarSaldoAsync(_currentUser.UserId, novoAporte.Valor);

        meta.RegistrarAporte(novoAporte);
        await _uof.CommitAsync();

        return ApiResponse<AporteMetasDTO>.Ok(_mapper.Map<AporteMetasDTO>(novoAporte), $"Aporte no valor de {novoAporte.Valor:C2} registrado com sucesso.");
    }

    public async Task<ApiResponse<AporteMetasDTO>> RemoverAporteAsync(Guid aporteMetaId)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.Aportes.Any(a => a.Id == aporteMetaId));

        if (meta is null)
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundMeta);

        var aporteRemovido = meta.Aportes.First(a => a.Id == aporteMetaId);

        if (aporteRemovido is null)
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundAporte);

        await _carteira.AdicionarSaldoAsync(_currentUser.UserId, aporteRemovido.Valor);
        meta.RemoverAporte(aporteRemovido);

        await _uof.CommitAsync();

        return ApiResponse<AporteMetasDTO>.Ok(_mapper.Map<AporteMetasDTO>(aporteRemovido));
    }

    public async Task<ApiResponse<MetaDTO>> RemoverMetaAsync(Guid metaId)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.MetaId == metaId);

        if (meta is null)
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);

        await _uof.MetaRepository.DeleteAsync(meta);

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

    public async Task<ApiResponse<decimal>> ValorTotalEmAportes(Guid metaId)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.MetaId == metaId);

        if (meta is null)
            return ApiResponse<decimal>.Fail(ResultMessages.NotFoundMeta);

        var totalAportes = meta.ValorTotalAportes();

        return ApiResponse<decimal>.Ok(totalAportes, $"Valor total investido na meta {totalAportes:C2}");
    }
}
