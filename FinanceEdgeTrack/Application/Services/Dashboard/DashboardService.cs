using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Consolidado;
using FinanceEdgeTrack.Domain.Interfaces.Metrics;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using FinanceEdgeTrack.Domain.Interfaces.Services.Cache;
using FinanceEdgeTrack.Domain.Interfaces.Services.Dashboard;

namespace FinanceEdgeTrack.Application.Services.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly ICarteiraMetrics _carteiraMetrics;
    private readonly IDespesaMetrics _despesaMetrics;
    private readonly IMetaMetrics _metaMetrics;
    private readonly IReceitaMetrics _receitaMetrics;
    private readonly ICurrentUser _currentUser;
    private readonly ICacheService _cacheService;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(ICarteiraMetrics carteiraMetrics, IDespesaMetrics despesaMetrics, IMetaMetrics metaMetrics,
                            IReceitaMetrics receitaMetrics, ICurrentUser currentUser, ICacheService cacheService,
                            ILogger<DashboardService> logger)
    {
        _carteiraMetrics = carteiraMetrics;
        _despesaMetrics = despesaMetrics;
        _metaMetrics = metaMetrics;
        _receitaMetrics = receitaMetrics;
        _currentUser = currentUser;
        _cacheService = cacheService;
        _logger = logger;
    }

    private string CacheKey()
    {
        return _cacheService.SetCacheKey(_currentUser.UserId);
    }

    private bool DateIsValid(int year, int month)
    {
        if (year >= 2026 || year <= 2040 && month >= 1 && month <= 12)
        {
            return true;
        }
        return false;
    }

    public async Task<ApiResponse<DashboardConsolidadoDTO>> GetDashboardGeral()
    {
        var cached = await _cacheService.TryGetAsync<DashboardConsolidadoDTO>(CacheKey());

        if (cached != null)
            return ApiResponse<DashboardConsolidadoDTO>.Ok(cached);

        var saldoTask = _carteiraMetrics.GetSaldoAtualUser();
        var despesaMetricsTask = _despesaMetrics.GetDespesaMetricsTotal();
        var receitaMetricsTask = _receitaMetrics.GetReceitaMetrics();
        var metaMetricsTask = _metaMetrics.GetMetricsMetas();
        var metaKpisTask = _metaMetrics.GetKPIsMetas();

        await Task.WhenAll(saldoTask, despesaMetricsTask, receitaMetricsTask, metaMetricsTask, metaKpisTask);

        var saldoAtual = saldoTask.Result;
        var despesaMetrics = despesaMetricsTask.Result;
        var receitaMetrics = receitaMetricsTask.Result;
        var metaMetrics = metaMetricsTask.Result;
        var metaKpis = metaKpisTask.Result;

        if (saldoAtual?.Data is null ||
        despesaMetrics?.Data is null ||
        receitaMetrics?.Data is null ||
        metaMetrics?.Data is null ||
        metaKpis?.Data is null)
        {
            return ApiResponse<DashboardConsolidadoDTO>.Fail(ResultMessages.ErrorToCreateDashboard);
        }

        var dashboardDTO = new DashboardConsolidadoDTO
        {
            Saldo = saldoAtual.Data.SaldoAtual,
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

        await _cacheService.SetAsync(CacheKey(), dashboardDTO, TimeSpan.FromMinutes(10));

        return ApiResponse<DashboardConsolidadoDTO>.Ok(dashboardDTO);
    }

    public async Task<ApiResponse<DashboardConsolidadoNoMesDTO>> GetDashboardMensal(int year, int month)
    {
        if (!DateIsValid(year, month))
            return ApiResponse<DashboardConsolidadoNoMesDTO>.Fail(ResultMessages.InvalidDateDashboard);

        var cached = await _cacheService.TryGetAsync<DashboardConsolidadoNoMesDTO>(CacheKey());

        if (cached != null)
            return ApiResponse<DashboardConsolidadoNoMesDTO>.Ok(cached);

        var daysInMonth = DateTime.DaysInMonth(year, month); // apenas para dividir os gasto pelos dias do mês.


        var saldoTask = _carteiraMetrics.GetSaldoAtualUser();
        var despesaMetricsTask = _despesaMetrics.GetDespesaMetricsNoMes(year, month);
        var receitaMetricsTask = _receitaMetrics.GetReceitaMetricsNoMes(year, month);
        var metaMetricsTask = _metaMetrics.GetMetricsMetasNoMes(year, month);
        var metaKpisTask = _metaMetrics.GetKPIsMetasNoMes(year, month);

        await Task.WhenAll(saldoTask, despesaMetricsTask, receitaMetricsTask, metaMetricsTask, metaKpisTask);

        var saldoAtual = saldoTask.Result;
        var despesaMetrics = despesaMetricsTask.Result;
        var receitaMetrics = receitaMetricsTask.Result;
        var metaMetrics = metaMetricsTask.Result;
        var metaKpis = metaKpisTask.Result;

        if (saldoAtual?.Data is null ||
        despesaMetrics?.Data is null ||
        receitaMetrics?.Data is null ||
        metaMetrics?.Data is null ||
        metaKpis?.Data is null)
        {
            return ApiResponse<DashboardConsolidadoNoMesDTO>.Fail(ResultMessages.ErrorToCreateDashboard);
        }

        var dashboardMensalDTO = new DashboardConsolidadoNoMesDTO
        {
            Saldo = saldoAtual.Data.SaldoAtual,
            MediaGastosDiarioNoMes = daysInMonth > 0 ? (despesaMetrics.Data.ValorTotalDespesasNoMes / daysInMonth) : 0,
            ValorTotalGastoNoMes = despesaMetrics.Data.ValorTotalDespesasNoMes,
            ValorTotalRecebidoNoMes = receitaMetrics.Data.ValorTotalReceitasNoMes,
            ValorAlvoEmMetasNoMes = metaMetrics.Data.TotalValorAlvoNoMes,
            ValorAportadoEmMetasNoMes = metaMetrics.Data.TotalAportadoNoMes,
            MediaDiasParaConclusao = (int)metaKpis.Data.MediaDiasParaCompletar,
            TotalConcluidasNoMes = metaKpis.Data.TotalConcluidasNoMes,
            TotalPendentesNoMes = metaKpis.Data.TotalPendentesNoMes,
            TotalCanceladasNoMes = metaKpis.Data.TotalCanceladasNoMes
        };

        await _cacheService.SetAsync(CacheKey(), dashboardMensalDTO, TimeSpan.FromMinutes(10));

        return ApiResponse<DashboardConsolidadoNoMesDTO>.Ok(dashboardMensalDTO);
    }

    public async Task<ApiResponse<DashboardConsolidadoPeriodoDTO>> GetDashboardPeriodo(DateTime dataInicio, DateTime dataFim)
    {
        var cached = await _cacheService.TryGetAsync<DashboardConsolidadoPeriodoDTO>(CacheKey());

        if (cached != null)
            return ApiResponse<DashboardConsolidadoPeriodoDTO>.Ok(cached);

        var diferenceInDays = dataInicio.Subtract(dataFim);
        int totalDays = (int)diferenceInDays.TotalDays;

        var saldoTask = _carteiraMetrics.GetSaldoAtualUser();
        var despesaMetricsTask = _despesaMetrics.GetDespesaMetricsNoPeriodo(dataInicio, dataFim);
        var receitaMetricsTask = _receitaMetrics.GetReceitaMetricsNoPeriodo(dataInicio, dataFim);
        var metaMetricsTask = _metaMetrics.GetMetricsMetasNoPeriodo(dataInicio, dataFim);
        var metaKpisTask = _metaMetrics.GetKPIsMetasNoPeriodo(dataInicio, dataFim);

        await Task.WhenAll(saldoTask, despesaMetricsTask, receitaMetricsTask, metaMetricsTask, metaKpisTask);

        var saldoAtual = saldoTask.Result;
        var despesaMetrics = despesaMetricsTask.Result;
        var receitaMetrics = receitaMetricsTask.Result;
        var metaMetrics = metaMetricsTask.Result;
        var metaKpis = metaKpisTask.Result;

        if (saldoAtual?.Data is null ||
        despesaMetrics?.Data is null ||
        receitaMetrics?.Data is null ||
        metaMetrics?.Data is null ||
        metaKpis?.Data is null)
        {
            return ApiResponse<DashboardConsolidadoPeriodoDTO>.Fail(ResultMessages.ErrorToCreateDashboard);
        }

        var dashboardPeriodoDTO = new DashboardConsolidadoPeriodoDTO
        {
            Saldo = saldoAtual.Data.SaldoAtual,
            MediaGastosDiarioNoPeriodo = (despesaMetrics.Data.ValorTotalDespesasNoPeriodo / totalDays),
            ValorTotalGastoNoPeriodo = despesaMetrics.Data.ValorTotalDespesasNoPeriodo,
            ValorTotalRecebidoNoPeriodo = receitaMetrics.Data.ValorTotalReceitasNoPeriodo,
            ValorAlvoEmMetasNoPeriodo = metaMetrics.Data.TotalValorAlvoNoPeriodo,
            ValorAportadoEmMetasNoPeriodo = metaMetrics.Data.TotalAportadoNoPeriodo,
            TotalConcluidasNoPeriodo = metaKpis.Data.TotalConcluidasNoPeriodo,
            TotalPendentesNoPeriodo = metaKpis.Data.TotalPendentesNoPeriodo,
            TotalCanceladasNoPeriodo = metaKpis.Data.TotalCanceladasNoPeriodo
        };

        await _cacheService.SetAsync(CacheKey(), dashboardPeriodoDTO, TimeSpan.FromMinutes(10));

        return ApiResponse<DashboardConsolidadoPeriodoDTO>.Ok(dashboardPeriodoDTO);
    }


}
