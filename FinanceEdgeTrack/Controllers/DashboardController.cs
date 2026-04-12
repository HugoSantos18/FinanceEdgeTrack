using Asp.Versioning;
using FinanceEdgeTrack.Domain.Interfaces.Services.Dashboard;
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
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet("mensal")]
    public async Task<IActionResult> GetDashboardMonth(int year, int month)
    {
        var response = await _dashboardService.GetDashboardMensal(year, month);

        if (!response.Success)
            return BadRequest();

        return Ok(response);
    }

    [HttpGet("general")]
    public async Task<IActionResult> GetDashboardGeneral()
    {
        var response = await _dashboardService.GetDashboardGeral();

        if(!response.Success)
            return BadRequest();

        return Ok(response);
    }

    [HttpGet("period")]
    public async Task<IActionResult> GetDashboardPeriod(DateTime dateStart, DateTime dateEnd)
    {
        var response = await _dashboardService.GetDashboardPeriodo(dateStart, dateEnd);

        if(!response.Success)
            return BadRequest();

        return Ok(response);
    }

}
