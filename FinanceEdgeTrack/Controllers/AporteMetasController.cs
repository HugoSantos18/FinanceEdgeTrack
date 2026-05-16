using Asp.Versioning;
using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Interfaces.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace FinanceEdgeTrack.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AporteMetasController : ControllerBase
{
    private readonly IAporteMetasService _aporteService;

    public AporteMetasController(IAporteMetasService aporteService)
    {
        _aporteService = aporteService;
    }

    [HttpGet("{aporteId:guid}", Name = "GetAporteMeta")]
    public async Task<IActionResult> GetByIdAsync(Guid aporteId)
    {
        var response = await _aporteService.GetAporteByIdAsync(aporteId);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("meta/{metaId:guid}", Name = "GetAportesDaMeta")]
    public async Task<IActionResult> GetAllByMetaAsync(Guid metaId, [FromQuery] PaginationParams pagination)
    {
        var response = await _aporteService.GetAportesDaMetaAsync(metaId, pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("meta/{metaId:guid}/total", Name = "GetTotalAportadoNaMeta")]
    public async Task<IActionResult> GetTotalDaMetaAsync(Guid metaId)
    {
        var response = await _aporteService.ValorTotalDaMetaAsync(metaId);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPost("meta/{metaId:guid}")]
    public async Task<IActionResult> PostAsync(Guid metaId, [FromBody] CreateAporteMetaDTO dto)
    {
        var response = await _aporteService.RegistrarAporteAsync(metaId, dto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{aporteId:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid aporteId)
    {
        var response = await _aporteService.RemoverAporteAsync(aporteId);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}
