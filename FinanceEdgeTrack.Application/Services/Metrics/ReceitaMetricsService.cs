using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Dashboard.Receitas;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Application.Interfaces.Services.Metrics;
using FinanceEdgeTrack.Application.Interfaces.Services.Auth;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Application.Services.Metrics;

public class ReceitaMetricsService : IReceitaMetrics
{
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _uof;
    private readonly ILogger<ReceitaMetricsService> _logger;

    public ReceitaMetricsService(ICurrentUser currentUser, IUnitOfWork uof, ILogger<ReceitaMetricsService> logger)
    {
        _currentUser = currentUser;
        _uof = uof;
        _logger = logger;
    }

    public async Task<ApiResponse<ReceitasGeralDTO>> GetReceitaMetrics()
    {
        var receitas = await _uof.ReceitaRepository
            .Query()
            .AsNoTracking()
            .ToListAsync();

        if (receitas.Count == 0)
        {
            _logger.LogInformation("Nenhuma receita registrada ainda");
            
            var receitaGeralEmptyDTO = new ReceitasGeralDTO
            {
                ValorTotalReceitas = 0
            };

            return ApiResponse<ReceitasGeralDTO>.Ok(receitaGeralEmptyDTO, $"Nenhuma receita registrada ainda");
        }

        decimal valorTotalReceitas = receitas
                                    .Sum(r => r.Valor);

        var receitaGeralDTO = new ReceitasGeralDTO
        {
            ValorTotalReceitas = valorTotalReceitas
        };

        return ApiResponse<ReceitasGeralDTO>.Ok(receitaGeralDTO, $"Valor total em receitas: R${receitaGeralDTO.ValorTotalReceitas:C2}");

    }

    public async Task<ApiResponse<ReceitasResumoMensalDTO>> GetReceitaMetricsNoMes(int year, int month)
    {
        var startDate = DateTime.SpecifyKind(new DateTime(year, month, 1), DateTimeKind.Utc);
        var endDate = DateTime.SpecifyKind(startDate.AddMonths(1).AddTicks(-1), DateTimeKind.Utc);

        var receitas = await _uof.ReceitaRepository
            .Query()
            .AsNoTracking()
            .Where(r => r.Data >= startDate && r.Data <= endDate)
            .ToListAsync();

        if (receitas.Count == 0)
        {
            _logger.LogInformation("Receitas no mês {Month}/{Year} ainda não registradas", month, year);
            
            var receitaResumoMensalEmptyDTO = new ReceitasResumoMensalDTO
            {
                ValorTotalReceitasNoMes = 0
            };
            
            return ApiResponse<ReceitasResumoMensalDTO>.Ok(receitaResumoMensalEmptyDTO, $"Nenhuma receita registrada ainda em {month}-{year}");
        }

        decimal valorReceitasTotalNoMes = receitas
                               .Sum(r => r.Valor);

        var receitaResumoMensalDTO = new ReceitasResumoMensalDTO
        {
            ValorTotalReceitasNoMes = valorReceitasTotalNoMes
        };


        return ApiResponse<ReceitasResumoMensalDTO>.Ok(receitaResumoMensalDTO, $"Receita do mês {month}");
    }


    public async Task<ApiResponse<ReceitasResumoPeriodoDTO>> GetReceitaMetricsNoPeriodo(DateTime start, DateTime end)
    {
        var startUtc = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(end, DateTimeKind.Utc);

        var receitas = await _uof.ReceitaRepository
            .Query()
            .AsNoTracking()
            .Where(r => r.Data >= startUtc && r.Data <= endUtc)
            .ToListAsync();

        if (receitas.Count == 0)
        {
            _logger.LogError("Erro ao buscar receitas no período: {Start} - {End}", start.ToShortDateString(), end.ToShortDateString());

            var receitaResumoPeriodoEmptyDTO = new ReceitasResumoPeriodoDTO
            {
                ValorTotalReceitasNoPeriodo = 0
            };

            return ApiResponse<ReceitasResumoPeriodoDTO>.Ok(receitaResumoPeriodoEmptyDTO, $"Nenhuma receita registrada ainda no período {start.ToShortDateString()} - {end.ToShortDateString()}");
        }

        decimal valorReceitasTotalNoPeriodo = receitas
                               .Sum(r => r.Valor);

        var receitaResumoPeriodoDTO = new ReceitasResumoPeriodoDTO
        {
            ValorTotalReceitasNoPeriodo = valorReceitasTotalNoPeriodo
        };


        return ApiResponse<ReceitasResumoPeriodoDTO>.Ok(receitaResumoPeriodoDTO, $"Receita do período {start.ToShortDateString()} | {end.ToShortDateString()}");
    }
}
