using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Pagination.Filters;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Enum;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using FinanceEdgeTrack.Domain.Interfaces.Services.CarteiraService;
using FinanceEdgeTrack.Domain.Interfaces.Services.Categories;
using FinanceEdgeTrack.Domain.Models;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Application.Services.Categories;

public class MetaService : IMetaService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _uof;
    private readonly ICarteiraService _carteiraService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<MetaService> _logger;

    public MetaService(IMapper mapper, IUnitOfWork uof, ICarteiraService carteiraService,
                       ICurrentUser currentUser, ILogger<MetaService> logger)
    {
        _mapper = mapper;
        _uof = uof;
        _carteiraService = carteiraService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ApiResponse<MetaDTO>> GetMetaPorIdAsync(Guid metaId)
    {
        var meta = await _uof.MetaRepository
                             .Query()
                             .Where(m => m.Carteira != null && m.Carteira!.UserId.Equals(_currentUser.UserId))
                             .FirstOrDefaultAsync(m => m.MetaId == metaId);

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível encontrar meta de ID: {metaId}, verifique o ID informado.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

    public async Task<ApiResponse<AporteMetasDTO>> GetAportePorIdAsync(Guid aporteMetaId)
    {
        var aporte = await _uof.MetaRepository
                               .Query()
                               .SelectMany(m => m.Aportes)
                               .Where(a => a.AporteMetasId == aporteMetaId)
                               .ProjectToType<AporteMetasDTO>()
                               .FirstOrDefaultAsync();

        if (aporte is null)
        {
            _logger.LogInformation($"Não foi possível encontrar aporte de ID: {aporteMetaId}, verifique o ID informado.");
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        return ApiResponse<AporteMetasDTO>.Ok(_mapper.Map<AporteMetasDTO>(aporte));
    }

    public async Task<ApiResponse<PagedList<AporteMetasDTO>>> GetAllAportesDaMetaPorIdAsync(Guid metaId, PaginationParams pagination)
    {
        var query = _uof.MetaRepository
        .Query()
        .Where(m => m.Carteira != null && m.Carteira!.UserId.Equals(_currentUser.UserId))
        .Where(m => m.MetaId == metaId)
        .SelectMany(m => m.Aportes)
        .OrderByDescending(a => a.Valor)
        .AsNoTracking()
        .ProjectToType<AporteMetasDTO>();

        var aportesPaginados = await PagedList<AporteMetasDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        return ApiResponse<PagedList<AporteMetasDTO>>.Ok(aportesPaginados);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> GetAllMetasAsync(PaginationParams pagination)
    {
        var query = _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira.UserId.Equals(_currentUser.UserId))
            .OrderByDescending(m => m.DataInicio)
            .AsNoTracking()
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaiorValorAsync(PaginationParams pagination)
    {
        var query = _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId.Equals(_currentUser.UserId))
            .OrderByDescending(m => m.ValorAlvo)
            .AsNoTracking()
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMenorValorAsync(PaginationParams pagination)
    {
        var query = _uof.MetaRepository
            .Query()
            .AsNoTracking()
            .OrderBy(m => m.ValorAlvo)
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasQuaseConcluidasAsync(PaginationParams pagination)
    {
        var query = _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId.Equals(_currentUser.UserId))
            .OrderByDescending(m => m.ValorAtual)
            .AsNoTracking()
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaisAntigaAsync(PaginationParams pagination)
    {
        var query = _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId.Equals(_currentUser.UserId))
            .OrderBy(m => m.DataInicio)
            .AsNoTracking()
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasMaisRecentesAsync(PaginationParams pagination)
    {
        var query = _uof.MetaRepository
             .Query()
             .Where(m => m.Carteira != null && m.Carteira!.UserId.Equals(_currentUser.UserId))
             .OrderByDescending(m => m.DataInicio)
             .AsNoTracking()
             .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            pagination.PageNumber,
            pagination.PageSize
            );

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> MetasFiltradasPorStatusAsync(StatusParams statusPagination)
    {
        var query = _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId.Equals(_currentUser.UserId))
            .Where(m => m.Status == statusPagination.Status)
            .AsNoTracking()
            .ProjectToType<MetaDTO>();

        var metasPaginadas = await PagedList<MetaDTO>.CreateAsync
            (
            query,
            statusPagination.PageNumber,
            statusPagination.PageSize
            );

        return ApiResponse<PagedList<MetaDTO>>.Ok(metasPaginadas);
    }

    public async Task<ApiResponse<MetaDTO>> CriarMetaAsync(CreateMetaDTO metaDto)
    {
        var carteira = await _carteiraService.GetCarteiraAsync();

        if (carteira is null)
        {
            _logger.LogWarning($"Carteira não encontrada para o User.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.WalletNotFound);
        }

        var meta = _mapper.Map<Meta>(metaDto);

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível criar a meta, verifique os dados informados.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.ValidMeta);
        }

        meta.CarteiraId = carteira.CarteiraId;
        carteira.Metas.Add(meta);

        await _uof.CommitAsync();

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

    public async Task<ApiResponse<MetaDTO>> AtualizarMetaAsync(Guid metaId, UpdateMetaDTO metaDto)
    {
        var meta = await _uof.MetaRepository
            .Query()
            .Where(m => m.Carteira != null && m.Carteira!.UserId.Equals(_currentUser.UserId))
            .FirstOrDefaultAsync(m => m.MetaId == metaId);

        if (meta is null)
        {
            _logger.LogWarning($"Meta de ID {metaId} não encontrada, verifique o ID informado.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        meta.Titulo = metaDto.Titulo;
        meta.ValorAlvo = metaDto.ValorAlvo;
        meta.AlterarDataAlvo(metaDto.DataAlvo);
        meta.AlterarStatus(metaDto.Status);
        metaDto.UpdatedAt = DateTime.UtcNow;

        await _uof.MetaRepository.UpdateAsync(meta);
        await _uof.CommitAsync();

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta), $"Meta {meta.Titulo} atualizada com sucesso");
    }

    public async Task<ApiResponse<MetaDTO>> RegistrarAporteAsync(Guid metaId, CreateAporteMetaDTO aporteMetaDto)
    {
        const int MAX_RETRY = 3;

        for (int attempt = 1; attempt <= MAX_RETRY; attempt++)
        {
            using var transaction = await _uof.BeginTransactionAsync();

            try
            {
                var carteira = await _carteiraService.GetCarteiraAsync();

                var meta = await _uof.MetaRepository
                                     .Query()
                                     .Include(m => m.Aportes)
                                     .Include(m => m.CarteiraId == carteira.CarteiraId)
                                     .FirstOrDefaultAsync(m => m.MetaId == metaId
                                     && carteira.UserId.Equals(_currentUser.UserId));

                if (meta is null)
                    return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);

                decimal saldo = carteira.Saldo;
                _logger.LogInformation($"Saldo atual: R${saldo:C2}");

                if (saldo < aporteMetaDto.Valor)
                    return ApiResponse<MetaDTO>.Fail("Saldo insuficiente para este aporte");

                carteira.DescontarSaldo(aporteMetaDto.Valor);

                var novoAporte = new AporteMetas
                {
                    AporteMetasId = Guid.NewGuid(),
                    MetaId = metaId,
                    Valor = aporteMetaDto.Valor
                };

                meta.RegistrarAporte(novoAporte);

                await _uof.CommitAsync();
                await transaction.CommitAsync();

                var metaDTO = _mapper.Map<MetaDTO>(meta);

                if (meta.Status == Status.Concluido)
                {
                    var dias = (meta.DataConclusao!.Value - meta.DataInicio).Days;

                    return ApiResponse<MetaDTO>.Ok(metaDTO,
                        $"🎉 Meta concluída em {dias} dias!");
                }


                return ApiResponse<MetaDTO>.Ok(metaDTO,
                    $"Aporte de {aporteMetaDto.Valor:C2} registrado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao registrar aporte na meta {metaId}");
                await transaction.RollbackAsync();
            }
        }

        return ApiResponse<MetaDTO>.Fail("Conflito de concorrência. Tente novamente.");
    }

    public async Task<ApiResponse<MetaDTO>> RemoverAporteAsync(Guid aporteMetaId)
    {
        var carteira = await _carteiraService.GetCarteiraAsync();

        var metas = _uof.MetaRepository
                        .Query()
                        .Where(m => m.CarteiraId == carteira.CarteiraId);

        var aporte = await metas
                               .SelectMany(m => m.Aportes)
                               .Where(a => a.AporteMetasId == aporteMetaId)
                               .FirstOrDefaultAsync();

        if (carteira is null)
        {
            _logger.LogWarning($"Carteira não encontrada para o User, verificar informações.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.WalletNotFound);
        }
        
        if (aporte is null)
        {
            _logger.LogWarning($"Não foi possível encontrar o aporte de ID {aporteMetaId}.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundAporte);
        }

        var meta = await metas
                       .Where(m => m.Aportes!.Contains(aporte))
                       .FirstOrDefaultAsync();
        
        if(meta is null)
        {
            _logger.LogWarning($"Não foi possível encontrar a meta de ID {meta?.MetaId}.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        carteira.AdicionarSaldo(aporte.Valor);
        meta.RemoverAporte(aporte);

        await _uof.CommitAsync();

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(metas), $"Aporte no valor de {aporte.Valor:C2} removido com sucesso.");
    }

    public async Task<ApiResponse<MetaDTO>> RemoverMetaAsync(Guid metaId)
    {
        var carteira = await _carteiraService.GetCarteiraAsync();

        var meta = await _uof.MetaRepository
                             .Query()
                             .Where(m => m.CarteiraId == carteira.CarteiraId)
                             .FirstOrDefaultAsync(m => m.MetaId == metaId);

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível remover meta, verifique o ID da meta {metaId} informado.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        carteira.Metas.Remove(meta);

        await _uof.CommitAsync();

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

    public async Task<ApiResponse<decimal>> ValorTotalEmAportes(Guid metaId)
    {
        var carteira = await _carteiraService.GetCarteiraAsync();

        var meta = carteira.Metas.FirstOrDefault(m => m.MetaId == metaId);

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível obter o total em aportes, verificar o ID da meta {metaId}.");
            return ApiResponse<decimal>.Fail(ResultMessages.NotFoundMeta);
        }

        var totalAportes = meta.ValorTotalAportes();

        return ApiResponse<decimal>.Ok(totalAportes, $"Valor total investido na meta {totalAportes:C2}");
    }
}
