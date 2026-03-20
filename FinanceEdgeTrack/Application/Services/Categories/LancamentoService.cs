using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Lancamentos;
using FinanceEdgeTrack.Application.Dtos.Write.Lancamentos;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Interfaces.Services.Categories;
using FinanceEdgeTrack.Domain.Models;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Application.Services.Categories;

public class LancamentoService : ILancamentoService
{
    private readonly IUnitOfWork _uof;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public LancamentoService(IUnitOfWork uof, ICurrentUserService currentUser, IMapper mapper)
    {
        _uof = uof;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ApiResponse<LancamentoDTO>> LancarAsync(LancamentoDTO lancamentoDTO)
    {
        var lancamento = _mapper.Map<Lancamento>(lancamentoDTO);

        if (lancamento is null)
            return ApiResponse<LancamentoDTO>.Fail(ResultMessages.ErrorCreation);

        await _uof.LancamentoRepository.CreateAsync(lancamento);

        return ApiResponse<LancamentoDTO>.Ok(_mapper.Map<LancamentoDTO>(lancamento));
    }

    public async Task<ApiResponse<LancamentoDTO>> AtualizarLancamentoAsync(Guid lancamentoId, UpdateLancamentoDTO lancamentoDto)
    {
        var lancamento = await _uof.LancamentoRepository.GetAsync(l => l.LancamentoId == lancamentoId);

        if (lancamento is null)
            return ApiResponse<LancamentoDTO>.Fail(ResultMessages.ErrorUpdate);

        lancamento.LancamentoId = lancamentoDto.LancamentoId;
        lancamento.DataLancamento = lancamentoDto.DataLancamento;
        lancamento.DespesaId = lancamentoDto.DespesaId;
        lancamento.ReceitaId = lancamentoDto.ReceitaId;
        lancamento.UserId = _currentUser.UserId;
        lancamentoDto.UpdatedAt = DateTime.UtcNow.ToShortDateString();

        await _uof.LancamentoRepository.UpdateAsync(lancamento);

        return ApiResponse<LancamentoDTO>.Ok(_mapper.Map<LancamentoDTO>(lancamento));
    }

    public async Task<ApiResponse<LancamentoDTO>> DeletarLancamentoAsync(Guid lancamentoId)
    {
        var lancamentoRemovido = await _uof.LancamentoRepository.GetAsync(l => l.LancamentoId == lancamentoId);

        if (lancamentoRemovido is null)
            return ApiResponse<LancamentoDTO>.Fail(ResultMessages.NotFoundLancamento);

        await _uof.LancamentoRepository.DeleteAsync(lancamentoRemovido);

        return ApiResponse<LancamentoDTO>.Ok(_mapper.Map<LancamentoDTO>(lancamentoRemovido));
    }

    public async Task<ApiResponse<LancamentoDTO>> GetByIdAsync(Guid lancamentoId)
    {
        var lancamento = await _uof.LancamentoRepository.GetAsync(l => l.LancamentoId == lancamentoId);

        if (lancamento is null)
            return ApiResponse<LancamentoDTO>.Fail(ResultMessages.NotFoundLancamento);

        return ApiResponse<LancamentoDTO>.Ok(_mapper.Map<LancamentoDTO>(lancamento));
    }

    public async Task<ApiResponse<PagedList<LancamentoDTO>>> GetAllLancamentosAsync(PaginationParams pagination)
    {
        var lancamentos = _uof.LancamentoRepository.GetAll();

        if (lancamentos is null)
            return ApiResponse<PagedList<LancamentoDTO>>.Fail(ResultMessages.NotFoundLancamento);

        var query = lancamentos
           .AsNoTracking()
           .OrderBy(l => l.DataLancamento)
           .ProjectToType<LancamentoDTO>();

        var lancamentosPaginados = await PagedList<LancamentoDTO>.CreateAsync
            (
             query,
             pagination.PageNumber,
             pagination.PageSize
            );


        return ApiResponse<PagedList<LancamentoDTO>>.Ok(lancamentosPaginados);
    }

    public async Task<ApiResponse<PagedList<LancamentoDTO>>> GetAllFilterByDataDescendingAsync(PaginationParams pagination)
    {
        var lancamentos = _uof.LancamentoRepository.GetAll();

        if (lancamentos is null)
            return ApiResponse<PagedList<LancamentoDTO>>.Fail(ResultMessages.NotFoundLancamento);

        var query = lancamentos
           .AsNoTracking()
           .OrderByDescending(l => l.DataLancamento)
           .ProjectToType<LancamentoDTO>();

        var lancamentosPaginados = await PagedList<LancamentoDTO>.CreateAsync
            (
             query,
             pagination.PageNumber,
             pagination.PageSize
            );


        return ApiResponse<PagedList<LancamentoDTO>>.Ok(lancamentosPaginados);
    }

}
