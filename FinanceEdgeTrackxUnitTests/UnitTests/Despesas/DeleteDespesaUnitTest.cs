using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Despesas;

public class DeleteDespesaUnitTest
{
    private readonly DespesaUnitTestController _helper;

    public DeleteDespesaUnitTest()
    {
        _helper = new DespesaUnitTestController();
    }

    [Fact]
    public async Task DeleteDespesa_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var id = Guid.NewGuid();
        var despesaDto = new DespesaDTO { DespesaId = id, Titulo = "Teste", Descricao = "Teste", Data = DateTime.UtcNow, Valor = 1000, Fixa = true };
        _helper.serviceMock
            .Setup(s => s.RemoverDespesaAsync(id))
            .ReturnsAsync(ApiResponse<DespesaDTO>.Ok(despesaDto));

        var result = await _helper.controller.Delete(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<DespesaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(despesaDto.DespesaId, response.Data!.DespesaId);
        _helper.serviceMock.Verify(s => s.RemoverDespesaAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteDespesa_ReturnsBadRequest_WhenServiceFails()
    {
        var id = Guid.NewGuid();
        var despesaDto = new DespesaDTO { DespesaId = id, Titulo = "Teste", Descricao = "Teste", Data = DateTime.UtcNow, Valor = 1000, Fixa = true };
        _helper.serviceMock
            .Setup(s => s.RemoverDespesaAsync(id))
            .ReturnsAsync(ApiResponse<DespesaDTO>.Fail(ResultMessages.NotFoundDespesa));

        var result = await _helper.controller.Delete(id);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<DespesaDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.RemoverDespesaAsync(id), Times.Once);
    }
}
