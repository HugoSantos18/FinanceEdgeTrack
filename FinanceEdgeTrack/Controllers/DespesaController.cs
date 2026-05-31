using Asp.Versioning;
using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Categorias;
using FinanceEdgeTrack.Application.DTOs.Write.Categorias;
using FinanceEdgeTrack.Application.Interfaces.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace FinanceEdgeTrack.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class DespesaController : ControllerBase
{
    private readonly IDespesaService _despesaService;

    public DespesaController(IDespesaService despesaService)
    {
        _despesaService = despesaService;
    }

    /// <summary>Retorna uma despesa pelo seu identificador único.</summary>
    /// <param name="id">GUID da despesa.</param>
    [HttpGet("{id}", Name = "GetDespesa")]
    [ProducesResponseType(typeof(ApiResponse<DespesaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var response = await _despesaService.ObterDespesaPorIdAsync(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    /// <summary>Lista todas as despesas do usuário autenticado com paginação.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("All")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DespesaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _despesaService.ListarDespesasAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Lista somente as despesas fixas do usuário autenticado com paginação.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("fixed")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DespesaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllFixasAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _despesaService.DespesasFixasPaginadasAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Lista as despesas ordenadas do maior para o menor valor.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("biggest-expense")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DespesaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllFilterByMostValueDescendingAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _despesaService.DespesasFiltradasMaiorValorAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Lista as despesas ordenadas do menor para o maior valor.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("lower-expense")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DespesaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllFilterByLessValueAscendingAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _despesaService.DespesasFiltradasMenorValorAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }


    /// <summary>Registra uma nova despesa para o usuário autenticado.</summary>
    /// <param name="despesaDto">Dados da despesa: Titulo, Descricao (opcional), Valor, Data e se é Fixa.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DespesaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Post([FromBody] CreateDespesaDTO despesaDto)
    {
        var response = await _despesaService.CreateDespesaAsync(despesaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Atualiza os dados de uma despesa existente.</summary>
    /// <param name="id">GUID da despesa a ser atualizada.</param>
    /// <param name="despesaDto">Campos que serão atualizados.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<DespesaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Put(Guid id, UpdateDespesaDTO despesaDto)
    {
        var response = await _despesaService.AtualizarDespesaAsync(id, despesaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Remove uma despesa pelo seu identificador único.</summary>
    /// <param name="id">GUID da despesa a ser removida.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _despesaService.RemoverDespesaAsync(id);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}
