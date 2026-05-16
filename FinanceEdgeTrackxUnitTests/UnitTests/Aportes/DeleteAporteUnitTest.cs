using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Aportes;

public class DeleteAporteUnitTest
{
    private readonly AporteMetasUnitTestController _helper;

    public DeleteAporteUnitTest()
    {
        _helper = new AporteMetasUnitTestController();
    }

    [Fact]
    public async Task DeleteAporte_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var aporteId = Guid.NewGuid();
        var metaId = Guid.NewGuid();
        var aporteResp = new AporteMetasDTO { MetaId = metaId, Valor = 350 };

        _helper.serviceMock
            .Setup(s => s.RemoverAporteAsync(aporteId))
            .ReturnsAsync(ApiResponse<AporteMetasDTO>.Ok(aporteResp, "Aporte removido com sucesso."));

        var result = await _helper.controller.DeleteAsync(aporteId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<AporteMetasDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(metaId, response.Data!.MetaId);
        Assert.Equal(350, response.Data!.Valor);
        _helper.serviceMock.Verify(s => s.RemoverAporteAsync(aporteId), Times.Once);
    }

    [Fact]
    public async Task DeleteAporte_ReturnsBadRequest_WhenAporteNaoEncontrado()
    {
        var aporteId = Guid.NewGuid();

        _helper.serviceMock
            .Setup(s => s.RemoverAporteAsync(aporteId))
            .ReturnsAsync(ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundAporte));

        var result = await _helper.controller.DeleteAsync(aporteId);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<AporteMetasDTO>>(bad.Value);
        Assert.False(response.Success);
        Assert.Equal(ResultMessages.NotFoundAporte, response.Message);
        _helper.serviceMock.Verify(s => s.RemoverAporteAsync(aporteId), Times.Once);
    }

    [Fact]
    public async Task DeleteAporte_ReturnsBadRequest_WhenMetaNaoPertenceAoUsuario()
    {
        var aporteId = Guid.NewGuid();

        _helper.serviceMock
            .Setup(s => s.RemoverAporteAsync(aporteId))
            .ReturnsAsync(ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundMeta));

        var result = await _helper.controller.DeleteAsync(aporteId);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<AporteMetasDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.RemoverAporteAsync(aporteId), Times.Once);
    }
}
