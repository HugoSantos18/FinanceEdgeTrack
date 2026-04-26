using Asp.Versioning;
using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Interfaces.Services.Categories;
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

    [HttpGet("{id}", Name = "GetDespesa")]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var response = await _despesaService.ObterDespesaPorIdAsync(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("All")]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _despesaService.ListarDespesasAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("fixas")]
    public async Task<IActionResult> GetAllFixasAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _despesaService.DespesasFixasPaginadasAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("maior-gasto")]
    public async Task<IActionResult> GetAllFilterByMostValueDescendingAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _despesaService.DespesasFiltradasMaiorValorAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("menor-gasto")]
    public async Task<IActionResult> GetAllFilterByLessValueAscendingAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _despesaService.DespesasFiltradasMenorValorAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateDespesaDTO despesaDto)
    {
        var response = await _despesaService.CreateDespesaAsync(despesaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response); 
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UpdateDespesaDTO despesaDto)
    {
        var response = await _despesaService.AtualizarDespesaAsync(id, despesaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _despesaService.RemoverDespesaAsync(id);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}
