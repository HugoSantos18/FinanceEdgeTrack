using Asp.Versioning;
using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Dtos.Read.Lancamentos;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Lancamentos;
using FinanceEdgeTrack.Application.Services;
using FinanceEdgeTrack.Domain.Interfaces.Services.Categories;
using Microsoft.AspNetCore.Mvc;

namespace FinanceEdgeTrack.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class LancamentoController : ControllerBase
{
    private readonly ILancamentoService _lancamentoService;
    private readonly ILogger<LancamentoController> _logger;

    public LancamentoController(ILancamentoService lancamentoService, ILogger<LancamentoController> logger)
    {
        _lancamentoService = lancamentoService;
        _logger = logger;
    }

    [HttpGet("{id}", Name = "GetLancamento")]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var response = await _lancamentoService.GetByIdAsync(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("All")]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _lancamentoService.GetAllLancamentosAsync(pagination);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("data-descending")]
    public async Task<IActionResult> GetAllFilterByDataDescendingAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _lancamentoService.GetAllFilterByDataDescendingAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> Post([FromBody] LancamentoDTO lancamentoDto)
    {
        var response = await _lancamentoService.LancarAsync(lancamentoDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UpdateLancamentoDTO updateLancamentoDto)
    {
        var response = await _lancamentoService.AtualizarLancamentoAsync(id, updateLancamentoDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _lancamentoService.DeletarLancamentoAsync(id);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}
