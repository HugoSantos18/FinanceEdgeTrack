using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Metas;
using FinanceEdgeTrack.Application.DTOs.Write.Categorias;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Aportes;

public class PostAporteUnitTest
{
    private readonly AporteMetasUnitTestController _helper;

    public PostAporteUnitTest()
    {
        _helper = new AporteMetasUnitTestController();
    }

    [Fact]
    public async Task PostAporte_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var metaId = Guid.NewGuid();
        var dto = new CreateAporteMetaDTO { Valor = 250 };
        var aporteResp = new AporteMetasDTO { MetaId = metaId, Valor = 250 };

        _helper.serviceMock
            .Setup(s => s.RegistrarAporteAsync(metaId, dto))
            .ReturnsAsync(ApiResponse<AporteMetasDTO>.Ok(aporteResp, "Aporte de R$ 250,00 registrado."));

        var result = await _helper.controller.PostAsync(metaId, dto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<AporteMetasDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(metaId, response.Data!.MetaId);
        Assert.Equal(250, response.Data!.Valor);
        _helper.serviceMock.Verify(s => s.RegistrarAporteAsync(metaId, dto), Times.Once);
    }

    [Fact]
    public async Task PostAporte_ReturnsBadRequest_WhenSaldoInsuficiente()
    {
        var metaId = Guid.NewGuid();
        var dto = new CreateAporteMetaDTO { Valor = 9999 };

        _helper.serviceMock
            .Setup(s => s.RegistrarAporteAsync(metaId, dto))
            .ReturnsAsync(ApiResponse<AporteMetasDTO>.Fail(ResultMessages.InsufficientBalance));

        var result = await _helper.controller.PostAsync(metaId, dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<AporteMetasDTO>>(bad.Value);
        Assert.False(response.Success);
        Assert.Equal(ResultMessages.InsufficientBalance, response.Message);
        _helper.serviceMock.Verify(s => s.RegistrarAporteAsync(metaId, dto), Times.Once);
    }

    [Fact]
    public async Task PostAporte_ReturnsBadRequest_WhenMetaNaoEncontrada()
    {
        var metaId = Guid.NewGuid();
        var dto = new CreateAporteMetaDTO { Valor = 100 };

        _helper.serviceMock
            .Setup(s => s.RegistrarAporteAsync(metaId, dto))
            .ReturnsAsync(ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundMeta));

        var result = await _helper.controller.PostAsync(metaId, dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<AporteMetasDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.RegistrarAporteAsync(metaId, dto), Times.Once);
    }

    [Fact]
    public async Task PostAporte_ReturnsBadRequest_WhenValorInvalido()
    {
        var metaId = Guid.NewGuid();
        var dto = new CreateAporteMetaDTO { Valor = 0 };

        _helper.serviceMock
            .Setup(s => s.RegistrarAporteAsync(metaId, dto))
            .ReturnsAsync(ApiResponse<AporteMetasDTO>.Fail(ResultMessages.MoreThanZero));

        var result = await _helper.controller.PostAsync(metaId, dto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<AporteMetasDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.RegistrarAporteAsync(metaId, dto), Times.Once);
    }
}
