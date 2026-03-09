using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FinanceEdgeTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DespesaController : ControllerBase
{
    private readonly IDespesaService _despesaService;
    private readonly ILogger<DespesaController> _logger;

    public DespesaController(IDespesaService despesaService, ILogger<DespesaController> logger)
    {
        _despesaService = despesaService;
        _logger = logger;
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
    public async Task<IActionResult> GetAllAsync()
    {
        var response = await _despesaService.ListarDespesasAsync();

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateDespesaDTO despesaDto)
    {
        var response = await _despesaService.CreateDespesaAsync(despesaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response); // deixar esse ou CreatedAtRouteResult (ver qual fica melhor)
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
