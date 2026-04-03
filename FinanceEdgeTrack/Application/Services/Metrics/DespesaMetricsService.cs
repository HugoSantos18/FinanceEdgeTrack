using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Despesas;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Metrics;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Application.Services.Metrics;

public class DespesaMetricsService : IDespesaMetrics
{
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uof;
    private readonly ILogger<DespesaMetricsService> _logger;

    public DespesaMetricsService(ICurrentUser currentUser, IUnitOfWork uof, ILogger<DespesaMetricsService> logger)
    {
        _currentUser = currentUser;
        _uof = uof;
        _logger = logger;
    }

    public async Task<ApiResponse<DespesasResumoMensalDTO>> GetDespesaMetricsNoMes(int month)
    {
        var query = _uof.DespesaRepository
            .Query()
            .AsNoTracking()
            .Where(d => d.Data.Month == month);

        var despesasDoMes = await query.ToListAsync();

        if (despesasDoMes.Count == 0)
            return ApiResponse<DespesasResumoMensalDTO>.Fail($"{ResultMessages.EmptyCollection}");

        decimal gastosFixos = despesasDoMes
                          .Where(d => d.Fixa)
                          .Sum(d => d.Valor);

        decimal gastoTotalNoMes = despesasDoMes
                              .Sum(d => d.Valor);

        decimal outrosGastos = despesasDoMes
                           .Where(d => !d.Fixa)
                           .Sum(d => d.Valor);

        var despesaResumoMensalDTO = new DespesasResumoMensalDTO
        {
            ValorTotalDespesasFixasNoMes = gastosFixos,
            ValorTotalDespesasNoMes = gastoTotalNoMes,
            ValorTotalOutrasDespesasNoMes = outrosGastos
        };

        return ApiResponse<DespesasResumoMensalDTO>.Ok(despesaResumoMensalDTO, $"Despesas do mês {month}");
    }

    public async Task<ApiResponse<DespesasGeralDTO>> GetDespesaMetricsTotal()
    {
        var query = _uof.DespesaRepository
                        .Query()
                        .AsNoTracking();


        var despesas = await query.ToListAsync();

        if (despesas is null)
            return ApiResponse<DespesasGeralDTO>.Fail($"{ResultMessages.NotFoundDespesa}");

        decimal gastosFixos = despesas
                              .Where(d => d.Fixa)
                              .Sum(d => d.Valor);

        decimal gastoTotal = despesas
                             .Sum(d => d.Valor);

        decimal outrosGastos = despesas
                           .Where(d => !d.Fixa)
                           .Sum(d => d.Valor);

        var despesaResumoMensalDTO = new DespesasGeralDTO
        {
            ValorTotalDespesasFixas = gastosFixos,
            ValorTotalDespesas = gastoTotal,
            ValorTotalOutrasDespesas = outrosGastos
        };

        return ApiResponse<DespesasGeralDTO>.Ok(despesaResumoMensalDTO);
    }
}
