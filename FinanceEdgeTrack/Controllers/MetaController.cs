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
public class MetaController : ControllerBase
{
    private readonly IMetaService _metaService;

    public MetaController(IMetaService metaService)
    {
        _metaService = metaService;
    }

    /// <summary>Retorna uma meta financeira pelo seu identificador único.</summary>
    /// <param name="id">GUID da meta.</param>
    [HttpGet("{id}", Name = "GetMeta")]
    [ProducesResponseType(typeof(ApiResponse<MetaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetMetaAsync(Guid id)
    {
        var response = await _metaService.GetMetaPorIdAsync(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    /// <summary>Lista todas as metas do usuário autenticado com paginação.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("All")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MetaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllMetasAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.GetAllMetasAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Lista as metas ordenadas do maior para o menor valor-alvo.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("biggest-expense")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MetaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllMetasFilterByMostValueAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.MetasFiltradasMaiorValorAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Lista as metas ordenadas do menor para o maior valor-alvo.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("lower-expense")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MetaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllMetasFilterByLesserValueAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.MetasFiltradasMenorValorAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Lista as metas com maior porcentagem de conclusão (quase concluídas primeiro).</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("almost-done")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MetaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllMetasFilterByAlmostDoneAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.MetasFiltradasQuaseConcluidasAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Lista as metas ordenadas das mais antigas para as mais recentes.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("oldest")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MetaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllMetasFilterByOldestAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.MetasFiltradasMaisAntigaAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Lista as metas ordenadas das mais recentes para as mais antigas.</summary>
    /// <param name="pagination">Parâmetros de paginação: PageNumber e PageSize (máx. 40).</param>
    [HttpGet("recently")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MetaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllMetasFilterByRecentlyAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.MetasFiltradasMaisRecentesAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Lista as metas filtradas por status (Pendente, Concluída ou Cancelada).</summary>
    /// <param name="statusPagination">Parâmetros de paginação e filtro de status.</param>
    [HttpGet("status")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MetaDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetAllMetasFilterByStatusAsync([FromQuery] StatusParams statusPagination)
    {
        var response = await _metaService.MetasFiltradasPorStatusAsync(statusPagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }


    /// <summary>Cria uma nova meta financeira para o usuário autenticado.</summary>
    /// <param name="metaDto">Dados da meta: Titulo, Descricao (opcional), ValorAlvo, DataInicio e DataAlvo.</param>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MetaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Post([FromBody] CreateMetaDTO metaDto)
    {
        var response = await _metaService.CriarMetaAsync(metaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }


    /// <summary>Atualiza os dados de uma meta financeira existente.</summary>
    /// <param name="id">GUID da meta a ser atualizada.</param>
    /// <param name="metaDto">Campos que serão atualizados: Titulo, ValorAlvo, DataAlvo e Status.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<MetaDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Put(Guid id, UpdateMetaDTO metaDto)
    {
        var response = await _metaService.AtualizarMetaAsync(id, metaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    /// <summary>Remove uma meta financeira pelo seu identificador único.</summary>
    /// <param name="id">GUID da meta a ser removida.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _metaService.RemoverMetaAsync(id);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

}
