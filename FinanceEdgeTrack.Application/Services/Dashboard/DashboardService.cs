using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Dashboard.Consolidado;
using FinanceEdgeTrack.Application.Interfaces.Services.Metrics;
using FinanceEdgeTrack.Application.Interfaces.Services.Auth;
using FinanceEdgeTrack.Application.Interfaces.Services.Cache;
using FinanceEdgeTrack.Application.Interfaces.Services.Carteira;
using FinanceEdgeTrack.Application.Interfaces.Services.Dashboard;

namespace FinanceEdgeTrack.Application.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly ICarteiraService _carteiraService;
    private readonly IDespesaMetrics _despesaMetrics;
    private readonly IMetaMetrics _metaMetrics;
    private readonly IReceitaMetrics _receitaMetrics;
    private readonly ICurrentUser _currentUser;
    private readonly ICacheService _cacheService;

    public DashboardService(ICarteiraService carteiraService, IDespesaMetrics despesaMetrics, IMetaMetrics metaMetrics,
                            IReceitaMetrics receitaMetrics, ICurrentUser currentUser, ICacheService cacheService)
    {
        _carteiraService = carteiraService;
        _despesaMetrics = despesaMetrics;
        _metaMetrics = metaMetrics;
        _receitaMetrics = receitaMetrics;
        _currentUser = currentUser;
        _cacheService = cacheService;
    }

    private string CacheKeyGeral() =>
        _cacheService.SetCacheKey($"{_currentUser.UserId}:dashboard:geral");

    private string CacheKeyMensal(int year, int month) =>
        _cacheService.SetCacheKey($"{_currentUser.UserId}:dashboard:mensal:{year}:{month}");

    private string CacheKeyPeriodo(DateTime start, DateTime end) =>
        _cacheService.SetCacheKey($"{_currentUser.UserId}:dashboard:periodo:{start:yyyyMMdd}:{end:yyyyMMdd}");

    private static bool DateIsValid(int year, int month)
    {
        return year >= 2000 && year <= DateTime.UtcNow.Year + 1 && month >= 1 && month <= 12;
    }

    public async Task<ApiResponse<DashboardConsolidadoDTO>> GetDashboardGeral()
    {
        var cached = await _cacheService.TryGetAsync<DashboardConsolidadoDTO>(CacheKeyGeral());
        if (cached != null)
            return ApiResponse<DashboardConsolidadoDTO>.Ok(cached);

        var carteira = await _carteiraService.GetCarteiraAsync();
        var despesaMetrics = await _despesaMetrics.GetDespesaMetricsTotal();
        var receitaMetrics = await _receitaMetrics.GetReceitaMetrics();
        var metaMetrics = await _metaMetrics.GetMetricsMetas();
        var metaKpis = await _metaMetrics.GetKPIsMetas();

        if (carteira is null ||
            despesaMetrics?.Data is null ||
            receitaMetrics?.Data is null ||
            metaMetrics?.Data is null ||
            metaKpis?.Data is null)
        {
            return ApiResponse<DashboardConsolidadoDTO>.Fail(ResultMessages.ErrorToCreateDashboard);
        }

        var dashboardDTO = new DashboardConsolidadoDTO
        {
            Saldo = carteira.Saldo,
            ValorTotalGasto = despesaMetrics.Data.ValorTotalDespesas,
            ValorTotalGastosFixos = despesaMetrics.Data.ValorTotalDespesasFixas,
            ValorTotalRecebido = receitaMetrics.Data.ValorTotalReceitas,
            ValorAlvoEmMetas = metaMetrics.Data.TotalValorAlvo,
            ValorAportadoEmMetas = metaMetrics.Data.TotalAportado,
            PorcentagemConcluida = metaMetrics.Data.PercentualConclusao,
            ValorRestanteParaCompletarTodas = metaKpis.Data.ValorTotalRestanteParaCompletarTodas,
            TotalConcluidas = metaKpis.Data.TotalConcluidas,
            TotalPendentes = metaKpis.Data.TotalPendentes,
            TotalCanceladas = metaKpis.Data.TotalCanceladas
        };

        await _cacheService.SetAsync(CacheKeyGeral(), dashboardDTO, TimeSpan.FromMinutes(10));

        return ApiResponse<DashboardConsolidadoDTO>.Ok(dashboardDTO);
    }

    public async Task<ApiResponse<DashboardConsolidadoNoMesDTO>> GetDashboardMensal(int year, int month)
    {
        if (!DateIsValid(year, month))
            return ApiResponse<DashboardConsolidadoNoMesDTO>.Fail(ResultMessages.InvalidDateDashboard);

        var cached = await _cacheService.TryGetAsync<DashboardConsolidadoNoMesDTO>(CacheKeyMensal(year, month));
        if (cached != null)
            return ApiResponse<DashboardConsolidadoNoMesDTO>.Ok(cached);

        var daysInMonth = DateTime.DaysInMonth(year, month);

        var carteira = await _carteiraService.GetCarteiraAsync();
        var despesaMetrics = await _despesaMetrics.GetDespesaMetricsNoMes(year, month);
        var receitaMetrics = await _receitaMetrics.GetReceitaMetricsNoMes(year, month);
        var metaMetrics = await _metaMetrics.GetMetricsMetasNoMes(year, month);
        var metaKpis = await _metaMetrics.GetKPIsMetasNoMes(year, month);

        if (carteira is null ||
            despesaMetrics?.Data is null ||
            receitaMetrics?.Data is null ||
            metaMetrics?.Data is null ||
            metaKpis?.Data is null)
        {
            return ApiResponse<DashboardConsolidadoNoMesDTO>.Fail(ResultMessages.ErrorToCreateDashboard);
        }

        var dashboardMensalDTO = new DashboardConsolidadoNoMesDTO
        {
            Saldo = carteira.Saldo,
            MediaGastosDiarioNoMes = daysInMonth > 0 ? despesaMetrics.Data.ValorTotalDespesasNoMes / daysInMonth : 0,
            ValorTotalGastoNoMes = despesaMetrics.Data.ValorTotalDespesasNoMes,
            ValorTotalRecebidoNoMes = receitaMetrics.Data.ValorTotalReceitasNoMes,
            ValorAlvoEmMetasNoMes = metaMetrics.Data.TotalValorAlvoNoMes,
            ValorAportadoEmMetasNoMes = metaMetrics.Data.TotalAportadoNoMes,
            MediaDiasParaConclusao = (int)metaKpis.Data.MediaDiasParaCompletar,
            TotalConcluidasNoMes = metaKpis.Data.TotalConcluidasNoMes,
            TotalPendentesNoMes = metaKpis.Data.TotalPendentesNoMes,
            TotalCanceladasNoMes = metaKpis.Data.TotalCanceladasNoMes
        };

        await _cacheService.SetAsync(CacheKeyMensal(year, month), dashboardMensalDTO, TimeSpan.FromMinutes(10));

        return ApiResponse<DashboardConsolidadoNoMesDTO>.Ok(dashboardMensalDTO);
    }

    public async Task<ApiResponse<DashboardConsolidadoPeriodoDTO>> GetDashboardPeriodo(DateTime dataInicio, DateTime dataFim)
    {
        var cached = await _cacheService.TryGetAsync<DashboardConsolidadoPeriodoDTO>(CacheKeyPeriodo(dataInicio, dataFim));
        if (cached != null)
            return ApiResponse<DashboardConsolidadoPeriodoDTO>.Ok(cached);

        int totalDays = (int)dataFim.Subtract(dataInicio).TotalDays;

        var carteira = await _carteiraService.GetCarteiraAsync();
        var despesaMetrics = await _despesaMetrics.GetDespesaMetricsNoPeriodo(dataInicio, dataFim);
        var receitaMetrics = await _receitaMetrics.GetReceitaMetricsNoPeriodo(dataInicio, dataFim);
        var metaMetrics = await _metaMetrics.GetMetricsMetasNoPeriodo(dataInicio, dataFim);
        var metaKpis = await _metaMetrics.GetKPIsMetasNoPeriodo(dataInicio, dataFim);


        if (carteira is null ||
            despesaMetrics?.Data is null ||
            receitaMetrics?.Data is null ||
            metaMetrics?.Data is null ||
            metaKpis?.Data is null)
        {
            return ApiResponse<DashboardConsolidadoPeriodoDTO>.Fail(ResultMessages.ErrorToCreateDashboard);
        }

        var dashboardPeriodoDTO = new DashboardConsolidadoPeriodoDTO
        {
            Saldo = carteira.Saldo,
            MediaGastosDiarioNoPeriodo = totalDays > 0 ? despesaMetrics.Data.ValorTotalDespesasNoPeriodo / totalDays : 0,
            ValorTotalGastoNoPeriodo = despesaMetrics.Data.ValorTotalDespesasNoPeriodo,
            ValorTotalRecebidoNoPeriodo = receitaMetrics.Data.ValorTotalReceitasNoPeriodo,
            ValorAlvoEmMetasNoPeriodo = metaMetrics.Data.TotalValorAlvoNoPeriodo,
            ValorAportadoEmMetasNoPeriodo = metaMetrics.Data.TotalAportadoNoPeriodo,
            TotalConcluidasNoPeriodo = metaKpis.Data.TotalConcluidasNoPeriodo,
            TotalPendentesNoPeriodo = metaKpis.Data.TotalPendentesNoPeriodo,
            TotalCanceladasNoPeriodo = metaKpis.Data.TotalCanceladasNoPeriodo
        };

        await _cacheService.SetAsync(CacheKeyPeriodo(dataInicio, dataFim), dashboardPeriodoDTO, TimeSpan.FromMinutes(10));

        return ApiResponse<DashboardConsolidadoPeriodoDTO>.Ok(dashboardPeriodoDTO);
    }
}
