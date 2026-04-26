
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Receitas;

public class DeleteReceitasUnitTest
{
    private readonly ReceitaUnitTestController _helper;

    public DeleteReceitasUnitTest()
    {
        _helper = new ReceitaUnitTestController();
    }

    [Fact]
    public async Task DeleteReceita_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var id = Guid.NewGuid();
        var receitaDto = new ReceitaDTO { ReceitaId = id, Titulo = "Teste", Descricao = "Teste", Data = DateTime.UtcNow, Valor = 1000 };
        _helper.serviceMock
            .Setup(s => s.RemoverReceitaAsync(id))
            .ReturnsAsync(ApiResponse<ReceitaDTO>.Ok(receitaDto));

        var result = await _helper.controller.Delete(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ReceitaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(receitaDto.ReceitaId, response.Data!.ReceitaId);
        _helper.serviceMock.Verify(s => s.RemoverReceitaAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteDespesa_ReturnsBadRequest_WhenServiceFails()
    {
        var id = Guid.NewGuid();
        var receitaDto = new ReceitaDTO { ReceitaId = id, Titulo = "Teste", Descricao = "Teste", Data = DateTime.UtcNow, Valor = 1000 };
        _helper.serviceMock
            .Setup(s => s.RemoverReceitaAsync(id))
            .ReturnsAsync(ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive));

        var result = await _helper.controller.Delete(id);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ReceitaDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.RemoverReceitaAsync(id), Times.Once);
    }
}
