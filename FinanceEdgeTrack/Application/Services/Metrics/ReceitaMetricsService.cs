using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Receitas;
using FinanceEdgeTrack.Application.Services.Auth;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Metrics;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Application.Services.Metrics;

public class ReceitaMetricsService : IReceitaMetrics
{
    private readonly CurrentUser _currentUser;
    private readonly IUnitOfWork _uof;
    private readonly ILogger<ReceitaMetricsService> _logger;

    public ReceitaMetricsService(CurrentUser currentUser, IUnitOfWork uof, ILogger<ReceitaMetricsService> logger)
    {
        _currentUser = currentUser;
        _uof = uof;
        _logger = logger;
    }

    public async Task<ApiResponse<ReceitasGeralDTO>> GetReceitaMetrics()
    {
        var query = _uof.ReceitaRepository
                       .Query()
                       .AsNoTracking();

        var receitas = await query.ToListAsync();

        if (receitas is null)
            return ApiResponse<ReceitasGeralDTO>.Fail(ResultMessages.EmptyCollection);


        decimal valorTotalReceitas = receitas
                                    .Sum(r => r.Valor);

        var receitaGeralDTO = new ReceitasGeralDTO
        {
            ValorTotalReceitas = valorTotalReceitas
        };

        return ApiResponse<ReceitasGeralDTO>.Ok(receitaGeralDTO, $"Valor total em receitas: R${receitaGeralDTO.ValorTotalReceitas:C2}");

    }

    public async Task<ApiResponse<ReceitasResumoMensalDTO>> GetReceitaMetricsNoMes(int month)
    {
        var query = _uof.ReceitaRepository
                          .Query()
                          .Where(r => r.Data.Month == month)
                          .AsNoTracking();

        var receitas = await query.ToListAsync();

        if (receitas is null)
            return ApiResponse<ReceitasResumoMensalDTO>.Fail(ResultMessages.EmptyCollection);

        decimal valorReceitasTotalNoMes = receitas
                               .Sum (r => r.Valor);

        var receitaResumoMensalDTO = new ReceitasResumoMensalDTO
        {
            ValorTotalReceitasNoMes = valorReceitasTotalNoMes
        };


        return ApiResponse<ReceitasResumoMensalDTO>.Ok(receitaResumoMensalDTO, $"Receita do mês {month}");
    }
}
