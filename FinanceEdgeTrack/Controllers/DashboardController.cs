using Asp.Versioning;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Dashboard.Consolidado;
using FinanceEdgeTrack.Application.Interfaces.Services.Dashboard;
using FinanceEdgeTrack.Infrastructure.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceEdgeTrack.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize(Policy = Role.Admin)]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>Retorna o dashboard consolidado de um mês específico. Inclui totais de gastos, receitas, metas e KPIs do período. Requer role <b>Admin</b>.</summary>
    /// <param name="year">Ano de referência (ex: 2025).</param>
    /// <param name="month">Mês de referência entre 1 e 12.</param>
    [HttpGet("monthly")]
    [ProducesResponseType(typeof(ApiResponse<DashboardConsolidadoNoMesDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetDashboardMonth(int year, int month)
    {
        var response = await _dashboardService.GetDashboardMensal(year, month);

        if (!response.Success)
            return BadRequest();

        return Ok(response);
    }

    /// <summary>Retorna o dashboard consolidado geral, agregando todos os dados históricos do usuário. Requer role <b>Admin</b>.</summary>
    [HttpGet("general")]
    [ProducesResponseType(typeof(ApiResponse<DashboardConsolidadoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetDashboardGeneral()
    {
        var response = await _dashboardService.GetDashboardGeral();

        if(!response.Success)
            return BadRequest();

        return Ok(response);
    }

    /// <summary>Retorna o dashboard consolidado para um intervalo de datas personalizado. Requer role <b>Admin</b>.</summary>
    /// <param name="dateStart">Data de início do período (formato: yyyy-MM-dd).</param>
    /// <param name="dateEnd">Data de fim do período (formato: yyyy-MM-dd).</param>
    [HttpGet("period")]
    [ProducesResponseType(typeof(ApiResponse<DashboardConsolidadoPeriodoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetDashboardPeriod(DateTime dateStart, DateTime dateEnd)
    {
        var response = await _dashboardService.GetDashboardPeriodo(dateStart, dateEnd);

        if(!response.Success)
            return BadRequest();

        return Ok(response);
    }

}
