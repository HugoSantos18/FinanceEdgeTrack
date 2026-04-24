using Asp.Versioning;
using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Pagination.Filters;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Interfaces.Services.Categories;
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

    [HttpGet("{id}", Name = "GetMeta")]
    public async Task<IActionResult> GetMetaAsync(Guid id)
    {
        var response = await _metaService.GetMetaPorIdAsync(id);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("aporte/{aporteId}", Name = "GetAporte")]
    public async Task<IActionResult> GetAporteAsync(Guid aporteId)
    {
        var response = await _metaService.GetAportePorIdAsync(aporteId);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }


    [HttpGet("All")]
    public async Task<IActionResult> GetAllMetasAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.GetAllMetasAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{metaId}/aportes", Name = "GetAllAportesFromMeta")]
    public async Task<IActionResult> GetAllAportesAsync(Guid metaId, [FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.GetAllAportesDaMetaPorIdAsync(metaId, pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("maior-valor")]
    public async Task<IActionResult> GetAllMetasFilterByMostValueAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.MetasFiltradasMaiorValorAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("menor-valor")]
    public async Task<IActionResult> GetAllMetasFilterByLesserValueAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.MetasFiltradasMenorValorAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("quase-feita")]
    public async Task<IActionResult> GetAllMetasFilterByAlmostDoneAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.MetasFiltradasQuaseConcluidasAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("antigas")]
    public async Task<IActionResult> GetAllMetasFilterByOldestAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.MetasFiltradasMaisAntigaAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("recentes")]
    public async Task<IActionResult> GetAllMetasFilterByRecentlyAsync([FromQuery] PaginationParams pagination)
    {
        var response = await _metaService.MetasFiltradasMaisRecentesAsync(pagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetAllMetasFilterByStatusAsync([FromQuery] StatusParams statusPagination)
    {
        var response = await _metaService.MetasFiltradasPorStatusAsync(statusPagination);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateMetaDTO metaDto)
    {
        var response = await _metaService.CriarMetaAsync(metaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }


    [HttpPost("{metaId}/aporte")]
    public async Task<IActionResult> PostAporte(Guid metaId, [FromBody] CreateAporteMetaDTO aporteDto)
    {
        var response = await _metaService.RegistrarAporteAsync(metaId, aporteDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, UpdateMetaDTO metaDto)
    {
        var response = await _metaService.AtualizarMetaAsync(id, metaDto);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("aporte/{aporteId}")]
    public async Task<IActionResult> DeleteAporte(Guid aporteId)
    {
        var response = await _metaService.RemoverAporteAsync(aporteId);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _metaService.RemoverMetaAsync(id);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }
}
