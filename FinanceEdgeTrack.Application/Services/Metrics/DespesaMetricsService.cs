using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Dashboard.Despesas;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Application.Interfaces.Services.Metrics;
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
        var startDate = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
        var endDate = DateTime.SpecifyKind(startDate.AddMonths(1).AddTicks(-1), DateTimeKind.Utc);

        var despesasDoMes = await _uof.DespesaRepository
            .Query()
            .AsNoTracking()
            .Where(d => d.Data >= startDate && d.Data <= endDate)
            .ToListAsync();

        if (despesasDoMes.Count == 0)
        {
            _logger.LogInformation($"Despesas no mês {startDate.Month.ToString()} ainda não registradas");

            var despesaResumoMensalEmptyDTO = new DespesasResumoMensalDTO
            {
                ValorTotalDespesasFixasNoMes = 0,
                ValorTotalDespesasNoMes = 0,
                ValorTotalOutrasDespesasNoMes = 0
            };

            return ApiResponse<DespesasResumoMensalDTO>.Ok(despesaResumoMensalEmptyDTO, $"Nenhuma despesa registrada ainda no mês {month}");
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
        var despesas = await _uof.DespesaRepository
            .Query()
            .AsNoTracking()
            .ToListAsync();

        if (despesas.Count == 0)
        {
            _logger.LogInformation($"Sem nenhuma despesa registrada ainda");

            var despesaResumoGeralEmptyDTO = new DespesasGeralDTO
            {
                ValorTotalDespesasFixas = 0,
                ValorTotalDespesas = 0,
                ValorTotalOutrasDespesas = 0
            };

            return ApiResponse<DespesasGeralDTO>.Ok(despesaResumoGeralEmptyDTO, $"Nenhuma despesa registrada ainda");
        }

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
        var startUtc = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(end, DateTimeKind.Utc);

        var despesas = await _uof.DespesaRepository
            .Query()
            .AsNoTracking()
            .Where(d => d.Data >= startUtc && d.Data <= endUtc)
            .ToListAsync();

        if (despesas.Count == 0)
        {
            _logger.LogError("Erro ao buscar despesas no período: {Start} - {End}", start.ToShortDateString(), end.ToShortDateString());

            var despesaResumoPeriodoEmptyDTO = new DespesasResumoPeriodoDTO
            {
                ValorTotalDespesasFixasNoPeriodo = 0,
                ValorTotalDespesasNoPeriodo = 0,
                ValorTotalOutrasDespesasNoPeriodo = 0
            };

            return ApiResponse<DespesasResumoPeriodoDTO>.Ok(despesaResumoPeriodoEmptyDTO, $"Nenhuma despesa registrada no período {start.ToShortDateString()} - {end.ToShortDateString()}");
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
