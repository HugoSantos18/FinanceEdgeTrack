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
    private readonly ICarteiraService _carteira;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<MetaService> _logger;

    public MetaService(IMapper mapper, IUnitOfWork uof, ICarteiraService carteira,
                       ICurrentUser currentUser, ILogger<MetaService> logger)
    {
        _mapper = mapper;
        _uof = uof;
        _carteira = carteira;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ApiResponse<MetaDTO>> GetMetaPorIdAsync(Guid metaId)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.MetaId == metaId);

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível encontrar meta de ID: {metaId}, verifique o ID informado.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

    public async Task<ApiResponse<AporteMetasDTO>> GetAportePorIdAsync(Guid aporteMetaId)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.Aportes.Any(a => a.AporteMetasId == aporteMetaId));

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível encontrar aporte de ID: {aporteMetaId}, verifique o ID informado.");
            return ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        var aporte = meta.Aportes.First(a => a.AporteMetasId == aporteMetaId);

        return ApiResponse<AporteMetasDTO>.Ok(_mapper.Map<AporteMetasDTO>(aporte));
    }

    public async Task<ApiResponse<PagedList<AporteMetasDTO>>> GetAllAportesDaMetaPorIdAsync(Guid metaId, PaginationParams pagination)
    {
        var query = _uof.MetaRepository
        .Query()
        .Where(m => m.MetaId == metaId)
        .SelectMany(m => m.Aportes)
        .OrderByDescending(a => a.Valor)
        .ProjectToType<AporteMetasDTO>();

        var aportesPaginados = await PagedList<AporteMetasDTO>.CreateAsync
            (
            query.Select(a => _mapper.Map<AporteMetasDTO>(a)),
            pagination.PageNumber,
            pagination.PageSize
            );

        return ApiResponse<PagedList<AporteMetasDTO>>.Ok(aportesPaginados);
    }

    public async Task<ApiResponse<PagedList<MetaDTO>>> GetAllMetasAsync(PaginationParams pagination)
    {
        var metas = _uof.MetaRepository.GetAll();

        if (metas is null)
        {
            _logger.LogInformation($"Não foi possível encontrar nenhuma meta, coleção possivelmente vazia.");
            return ApiResponse<PagedList<MetaDTO>>.Fail(ResultMessages.NotFoundMeta);
        }

        var query = metas
            .AsNoTracking()
            .OrderByDescending(m => m.DataInicio)
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
        var metas = _uof.MetaRepository.GetAll();

        if (metas is null)
        {
            _logger.LogInformation($"Não foi possível encontrar nenhuma meta, coleção possivelmente vazia.");
            return ApiResponse<PagedList<MetaDTO>>.Fail(ResultMessages.NotFoundMeta);
        }

        var query = metas
            .AsNoTracking()
            .OrderByDescending(m => m.ValorAlvo)
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
        var metas = _uof.MetaRepository.GetAll();

        if (metas is null)
        {
            _logger.LogInformation($"Não foi possível encontrar nenhuma meta, coleção possivelmente vazia.");
            return ApiResponse<PagedList<MetaDTO>>.Fail(ResultMessages.NotFoundMeta);
        }

        var query = metas
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
        var metas = _uof.MetaRepository.GetAll();

        if (metas is null)
        {
            _logger.LogInformation($"Não foi possível encontrar nenhuma meta, coleção possivelmente vazia.");
            return ApiResponse<PagedList<MetaDTO>>.Fail(ResultMessages.NotFoundMeta);
        }

        var query = metas
            .AsNoTracking()
            .OrderByDescending(m => m.ValorAtual)
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
        var metas = _uof.MetaRepository.GetAll();

        if (metas is null)
        {
            _logger.LogInformation($"Não foi possível encontrar nenhuma meta, coleção possivelmente vazia.");
            return ApiResponse<PagedList<MetaDTO>>.Fail(ResultMessages.NotFoundMeta);
        }

        var query = metas
            .AsNoTracking()
            .OrderBy(m => m.DataInicio)
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
        var metas = _uof.MetaRepository.GetAll();

        if (metas is null)
        {
            _logger.LogInformation($"Não foi possível encontrar nenhuma meta, coleção possivelmente vazia.");
            return ApiResponse<PagedList<MetaDTO>>.Fail(ResultMessages.NotFoundMeta);
        }

        var query = metas
            .AsNoTracking()
            .OrderByDescending(m => m.DataInicio)
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
        var metas = _uof.MetaRepository.GetAll();

        if (metas is null)
        {
            _logger.LogInformation($"Não foi possível encontrar nenhuma meta, coleção possivelmente vazia.");
            return ApiResponse<PagedList<MetaDTO>>.Fail(ResultMessages.NotFoundMeta);
        }

        var query = metas
            .AsNoTracking()
            .Where(m => m.Status == statusPagination.Status)
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
        // TODO FIX BUGS E ERROS DE CARTEIRA E RELACIONAMENTO DE APORTES.

        var meta = _mapper.Map<Meta>(metaDto);

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível criar a meta, verifique os dados informados.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.ValidMeta);
        }

        await _uof.MetaRepository.CreateAsync(meta);

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

    public async Task<ApiResponse<MetaDTO>> AtualizarMetaAsync(Guid metaId, UpdateMetaDTO metaDto)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.MetaId == metaId);

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível atualizar a meta {metaId}, verifique os dados informados.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        meta.Titulo = metaDto.Titulo;
        meta.ValorAlvo = metaDto.ValorAlvo;
        meta.AlterarDataAlvo(metaDto.DataAlvo);
        meta.AlterarStatus(metaDto.Status);
        metaDto.UpdatedAt = DateTime.UtcNow;

        await _uof.MetaRepository.UpdateAsync(meta);

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta), $"Meta {meta.Titulo} atualizada com sucesso");
    }

    public async Task<int> AtualizarValorAtualAsync(Guid metaId, decimal novoValor, decimal valorAntigoEsperado)
    {
        return await _uof.MetaRepository
            .Query()
            .Where(m => m.MetaId == metaId && m.ValorAtual == valorAntigoEsperado)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.ValorAtual, novoValor)
                .SetProperty(m => m.DataUltimoDeposito, DateTime.UtcNow));
    }

    public async Task<ApiResponse<MetaDTO>> RegistrarAporteAsync(Guid metaId, CreateAporteMetaDTO aporteMetaDto)
    {
        const int MAX_RETRY = 3;

        for ( int attempt = 1; attempt <= MAX_RETRY; attempt++)
        {
            using var transaction = await _uof.BeginTransactionAsync();

            try
            {
                var meta = await _uof.MetaRepository
                    .Query()
                    .FirstOrDefaultAsync(m => m.MetaId == metaId);

                if (meta is null)
                    return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);

                var response = await _carteira.ObterSaldoAsync(_currentUser.UserId);
                _logger.LogInformation($"Saldo atual: R${response.Data:C2}");

                if (response.Data < aporteMetaDto.Valor)
                    return ApiResponse<MetaDTO>.Fail("Saldo insuficiente para este aporte");

                decimal novoValorAtual = meta.ValorAtual + aporteMetaDto.Valor;

                if (novoValorAtual > meta.ValorAlvo)
                    novoValorAtual = meta.ValorAlvo;

                var rowsAffected = await AtualizarValorAtualAsync(
                    metaId,
                    novoValorAtual,
                    meta.ValorAtual);

                if (rowsAffected == 0)
                {
                    _logger.LogWarning($"Conflito na meta {metaId}, tentativa {attempt}/{MAX_RETRY}");
                    await transaction.RollbackAsync();
                    continue;
                }

                var novoAporte = new AporteMetas
                {
                    AporteMetasId = Guid.NewGuid(),
                    MetaId = metaId,
                    Valor = aporteMetaDto.Valor
                };

                meta.RegistrarAporte(novoAporte);

                await _carteira.DescontarSaldoAsync(aporteMetaDto.Valor, _currentUser.UserId);

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
        var meta = await _uof.MetaRepository.GetAsync(m => m.Aportes
                                            .Any(a => a.AporteMetasId == aporteMetaId));

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível encontrar meta, verifique o ID informado do aporte.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        var aporteRemovido = meta.Aportes.First(a => a.AporteMetasId == aporteMetaId);

        if (aporteRemovido is null)
        {
            _logger.LogInformation($"Não foi possível encontrar o aporte de ID {aporteMetaId}.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundAporte);
        }

        await _carteira.AdicionarSaldoAsync(aporteRemovido.Valor, _currentUser.UserId);
        meta.RemoverAporte(aporteRemovido);

        await _uof.CommitAsync();

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta), $"Aporte no valor de {aporteRemovido.Valor:C2} removido com sucesso.");
    }

    public async Task<ApiResponse<MetaDTO>> RemoverMetaAsync(Guid metaId)
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.MetaId == metaId);

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível remover meta, verifique o ID da meta {metaId} informado.");
            return ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta);
        }

        await _uof.MetaRepository.DeleteAsync(meta);

        return ApiResponse<MetaDTO>.Ok(_mapper.Map<MetaDTO>(meta));
    }

    public async Task<ApiResponse<decimal>> ValorTotalEmAportes(Guid metaId) 
    {
        var meta = await _uof.MetaRepository.GetAsync(m => m.MetaId == metaId);

        if (meta is null)
        {
            _logger.LogInformation($"Não foi possível obter o total em aportes, verificar o ID da meta {metaId}.");
            return ApiResponse<decimal>.Fail(ResultMessages.NotFoundMeta);
        }

        var totalAportes = meta.ValorTotalAportes();

        return ApiResponse<decimal>.Ok(totalAportes, $"Valor total investido na meta {totalAportes:C2}");
    }
}
