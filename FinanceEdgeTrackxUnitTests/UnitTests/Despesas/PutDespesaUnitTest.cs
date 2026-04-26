using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Despesas;

public class PutDespesaUnitTest
{
    private readonly DespesaUnitTestController _helper;

    public PutDespesaUnitTest()
    {
        _helper = new DespesaUnitTestController();
    }

    [Fact]
    public async Task PutDespesa_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateDespesaDTO { DespesaId = id, Titulo = "x", Valor = 100, Data = DateTime.UtcNow, Fixa = true };
        var mapped = _helper.mapper.Map<DespesaDTO>(dto);

        _helper.serviceMock
            .Setup(s => s.AtualizarDespesaAsync(id, It.IsAny<UpdateDespesaDTO>()))
            .ReturnsAsync(ApiResponse<DespesaDTO>.Ok(mapped));

        var result = await _helper.controller.Put(id, dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<DespesaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(mapped.DespesaId, response.Data!.DespesaId);
        _helper.serviceMock.Verify(s => s.AtualizarDespesaAsync(id, It.IsAny<UpdateDespesaDTO>()), Times.Once);
    }

    [Fact]
    public async Task PutDespesa_ReturnsBadRequest_WhenServiceFails()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateDespesaDTO { DespesaId = id, Titulo = "x", Valor = 100, Data = DateTime.UtcNow, Fixa = true };

        _helper.serviceMock
            .Setup(s => s.AtualizarDespesaAsync(id, It.IsAny<UpdateDespesaDTO>()))
            .ReturnsAsync(ApiResponse<DespesaDTO>.Fail(ResultMessages.NotFoundDespesa));

        var result = await _helper.controller.Put(id, dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<DespesaDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.AtualizarDespesaAsync(id, It.IsAny<UpdateDespesaDTO>()), Times.Once);
    }
}
