using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Receitas;

public class PutReceitasUnitTest
{
    private readonly ReceitaUnitTestController _helper;

    public PutReceitasUnitTest()
    {
        _helper = new ReceitaUnitTestController();
    }

    [Fact]
    public async Task PutReceita_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var id = Guid.NewGuid();
        var receitaDto = new UpdateReceitaDTO { ReceitaId = id, Titulo = "Teste tittle", Descricao = "Test description", Data = DateTime.UtcNow, Valor = 1000 };
        var mapped = _helper.mapper.Map<ReceitaDTO>(receitaDto);
        _helper.serviceMock
            .Setup(s => s.AtualizarReceitaAsync(id, It.IsAny<UpdateReceitaDTO>()))
            .ReturnsAsync(ApiResponse<ReceitaDTO>.Ok(mapped));

        var result = await _helper.controller.Put(id, receitaDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ReceitaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(mapped.ReceitaId, response.Data!.ReceitaId);
        _helper.serviceMock.Verify(s => s.AtualizarReceitaAsync(id, It.IsAny<UpdateReceitaDTO>()), Times.Once);
    }

    [Fact]
    public async Task PutReceita_ReturnsBadRequest_WhenServiceFails()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateReceitaDTO { ReceitaId = id, Titulo = "x",Descricao = "Descrição ", Valor = 100, Data = DateTime.UtcNow };

        _helper.serviceMock
            .Setup(s => s.AtualizarReceitaAsync(id, It.IsAny<UpdateReceitaDTO>()))
            .ReturnsAsync(ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive));

        var result = await _helper.controller.Put(id, dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ReceitaDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.AtualizarReceitaAsync(id, It.IsAny<UpdateReceitaDTO>()), Times.Once);
    }

}
