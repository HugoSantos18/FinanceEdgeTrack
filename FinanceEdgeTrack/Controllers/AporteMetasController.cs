using Asp.Versioning;
using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Metas;
using FinanceEdgeTrack.Application.DTOs.Write.Categorias;
using FinanceEdgeTrack.Application.Interfaces.Services.Categories;
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

    /// <summary>Retorna um aporte específico pelo seu identificador único.</summary>
    /// <param name="aporteId">GUID do aporte.</param>
    [HttpGet("{aporteId:guid}", Name = "GetAporteMeta")]
    [ProducesResponseType(typeof(ApiResponse<AporteMetasDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetByIdAsync(Guid aporteId)
    {
        var response = await _aporteService.GetAporteByIdAsync(aporteId);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    /// <summary>Lista todos os aportes registrados em uma meta específica com paginação.</summary>
    /// <param name="metaId">GUID da meta cujos aportes serão listados.</param>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("meta/{metaId:guid}", Name = "GetAportesDaMeta")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AporteMetasDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllByMetaAsync(Guid metaId, [FromQuery] PaginationParams pagination)
    {
        var response = await _aporteService.GetAportesDaMetaAsync(metaId, pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Retorna o valor total aportado em uma meta específica.</summary>
    /// <param name="metaId">GUID da meta.</param>
    [HttpGet("meta/{metaId:guid}/total", Name = "GetTotalAportadoNaMeta")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetTotalDaMetaAsync(Guid metaId)
    {
        var response = await _aporteService.ValorTotalDaMetaAsync(metaId);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    /// <summary>Registra um novo aporte em uma meta financeira.</summary>
    /// <param name="metaId">GUID da meta que receberá o aporte.</param>
    /// <param name="dto">Valor do aporte (entre R$ 1,00 e R$ 99.999.999,00).</param>
    [HttpPost("meta/{metaId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AporteMetasDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> PostAsync(Guid metaId, [FromBody] CreateAporteMetaDTO dto)
    {
        var response = await _aporteService.RegistrarAporteAsync(metaId, dto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Remove um aporte pelo seu identificador único.</summary>
    /// <param name="aporteId">GUID do aporte a ser removido.</param>
    [HttpDelete("{aporteId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteAsync(Guid aporteId)
    {
        var response = await _aporteService.RemoverAporteAsync(aporteId);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}
