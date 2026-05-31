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
public class ReceitaController : ControllerBase
{
    private readonly IReceitaService _receitaService;

    public ReceitaController(IReceitaService receitaService)
    {
        _receitaService = receitaService;
    }

    /// <summary>Retorna uma receita pelo seu identificador único.</summary>
    /// <param name="id">GUID da receita.</param>
    [HttpGet("{id}", Name = "GetReceita")]
    [ProducesResponseType(typeof(ApiResponse<ReceitaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var response = await _receitaService.ObterReceitaPorIdAsync(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);

    }

    /// <summary>Lista todas as receitas do usuário autenticado com paginação.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("All")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReceitaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _receitaService.ListarReceitasAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Lista as receitas ordenadas do maior para o menor valor.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("biggest-expense")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReceitaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllFilterByValueDescendingAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _receitaService.ReceitasFiltradasMaiorValorAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Lista as receitas ordenadas do menor para o maior valor.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("lower-expense")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReceitaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllFilterByValueAscendingAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _receitaService.ReceitasFiltradasMenorValorAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }


    /// <summary>Registra uma nova receita para o usuário autenticado.</summary>
    /// <param name="receitaDto">Dados da receita: Titulo, Descricao (opcional), Valor e Data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReceitaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Post([FromBody] CreateReceitaDTO receitaDto)
    {
        var response = await _receitaService.CreateReceitaAsync(receitaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Atualiza os dados de uma receita existente.</summary>
    /// <param name="id">GUID da receita a ser atualizada.</param>
    /// <param name="receitaDto">Campos que serão atualizados.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ReceitaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Put(Guid id, UpdateReceitaDTO receitaDto)
    {
        var response = await _receitaService.AtualizarReceitaAsync(id, receitaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Remove uma receita pelo seu identificador único.</summary>
    /// <param name="id">GUID da receita a ser removida.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _receitaService.RemoverReceitaAsync(id);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

}
