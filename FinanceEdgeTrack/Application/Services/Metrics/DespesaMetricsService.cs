using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Despesas;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Metrics;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Application.Services.Metrics;

public class DespesaMetricsService : IDespesaMetrics
{
    private readonly IUnitOfWork _uof;
    private readonly ILogger<DespesaMetricsService> _logger;

    public DespesaMetricsService(IUnitOfWork uof, ILogger<DespesaMetricsService> logger)
    {
        _uof = uof;
        _logger = logger;
    }

    public async Task<ApiResponse<DespesasResumoMensalDTO>> GetDespesaMetricsNoMes(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(1);

        var query = _uof.DespesaRepository
            .Query()
            .AsNoTracking()
            .Where(d => d.Data >= startDate && d.Data <= endDate);

        var despesasDoMes = await query.ToListAsync();

        if (despesasDoMes.Count == 0)
        {
            _logger.LogInformation($"Despesas no mês {startDate.Month.ToString()} ainda não registradas");
            return ApiResponse<DespesasResumoMensalDTO>.Fail($"{ResultMessages.EmptyCollection}");
        }

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

        var despesaResumoGeralDTO = new DespesasGeralDTO
        {
            ValorTotalDespesasFixas = gastosFixos,
            ValorTotalDespesas = gastoTotal,
            ValorTotalOutrasDespesas = outrosGastos
        };

        return ApiResponse<DespesasGeralDTO>.Ok(despesaResumoGeralDTO);
    }

    public async Task<ApiResponse<DespesasResumoPeriodoDTO>> GetDespesaMetricsNoPeriodo(DateTime start, DateTime end)
    {
        var query = _uof.DespesaRepository
                       .Query()
                       .Where(d => d.Data >= start)
                       .Where(d => d.Data <= end)
                       .AsNoTracking();


        var despesas = await query.ToListAsync();

        if (despesas is null)
        {
            _logger.LogError($"Erro ao buscar despesas no perído: {start.ToShortDateString()} - {end.ToShortDateString()}.", despesas);
            return ApiResponse<DespesasResumoPeriodoDTO>.Fail($"{ResultMessages.NotFoundDespesa}");

        }

        decimal gastosFixos = despesas
                              .Where(d => d.Fixa)
                              .Sum(d => d.Valor);

        decimal gastoTotal = despesas
                             .Sum(d => d.Valor);

        decimal outrosGastos = despesas
                           .Where(d => !d.Fixa)
                           .Sum(d => d.Valor);

        var despesaResumoPeriodoDTO = new DespesasResumoPeriodoDTO
        {
            ValorTotalDespesasFixasNoPeriodo = gastosFixos,
            ValorTotalDespesasNoPeriodo = gastoTotal,
            ValorTotalOutrasDespesasNoPeriodo = outrosGastos
        };

        return ApiResponse<DespesasResumoPeriodoDTO>.Ok(despesaResumoPeriodoDTO);
    }
}
