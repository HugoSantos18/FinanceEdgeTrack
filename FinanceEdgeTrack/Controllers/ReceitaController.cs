using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Application.Services;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceEdgeTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceitaController : ControllerBase
{
    private readonly IReceitaService _receitaService;
    private readonly ILogger<ReceitaController> _logger;


    public ReceitaController(IReceitaService receitaService, ILogger<ReceitaController> logger)
    {
        _receitaService = receitaService;
        _logger = logger;
    }

    [HttpGet("{id}", Name = "GetReceita")]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var response = await _receitaService.ObterReceitaPorIdAsync(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("All")]
    public async Task<IActionResult> GetAllAsync()
    {
        var response = await _receitaService.ListarReceitasAsync();

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateReceitaDTO receitaDto)
    {
        var response = await _receitaService.CreateReceitaAsync(receitaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UpdateReceitaDTO receitaDto)
    {
        var response = await _receitaService.AtualizarReceitaAsync(id, receitaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _receitaService.RemoverReceitaAsync(id);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

}
