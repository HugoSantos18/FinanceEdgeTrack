using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Domain.Enum;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Metas;

public class DeleteMetaUnitTest
{
    private readonly MetaUnitTestController _helper;

    public DeleteMetaUnitTest()
    {
        _helper = new MetaUnitTestController();
    }

    [Fact]
    public async Task DeleteMeta_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var id = Guid.NewGuid();
        var metaDto = new MetaDTO { MetaId = id, Titulo = "Test tittle", Descricao = "Test description", DataAlvo = DateTime.UtcNow.AddDays(4), DataInicio = DateTime.Now, PorcentagemAtual = 0, Status = Status.EmAberto, ValorAlvo = 10000 };
        _helper.serviceMock
            .Setup(s => s.RemoverMetaAsync(id))
            .ReturnsAsync(ApiResponse<MetaDTO>.Ok(metaDto));

        var result = await _helper.controller.Delete(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(metaDto.MetaId, response.Data!.MetaId);
        _helper.serviceMock.Verify(s => s.RemoverMetaAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteAporteMeta_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var aporteId = Guid.NewGuid();
        var metaId = Guid.NewGuid();
        var metaDto = new MetaDTO { MetaId = metaId, Titulo = "Test tittle", Descricao = "Test description", DataAlvo = DateTime.UtcNow.AddDays(4), DataInicio = DateTime.Now, PorcentagemAtual = 0, Status = Status.EmAberto, ValorAlvo = 10000 };
        var aporteDto = new AporteMetasDTO
        {
            MetaId = metaId,
            Valor = 350
        };
        _helper.serviceMock
            .Setup(s => s.RemoverAporteAsync(aporteId))
            .ReturnsAsync(ApiResponse<MetaDTO>.Ok(metaDto));

        var result = await _helper.controller.DeleteAporte(aporteId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(metaDto.MetaId, response.Data!.MetaId);
        _helper.serviceMock.Verify(s => s.RemoverAporteAsync(aporteId), Times.Once);
    }

    [Fact]
    public async Task DeleteMeta_ReturnsBadRequest_WhenServiceFails()
    {
        var metaId = Guid.NewGuid();
        var metaDto = new MetaDTO { MetaId = metaId, Titulo = "Test tittle", Descricao = "Test description", DataAlvo = DateTime.UtcNow.AddDays(4), DataInicio = DateTime.Now, PorcentagemAtual = 0, Status = Status.EmAberto, ValorAlvo = 10000 };
        _helper.serviceMock
             .Setup(s => s.RemoverMetaAsync(metaId))
             .ReturnsAsync(ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta));


        var result = await _helper.controller.Delete(metaId);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.RemoverMetaAsync(metaId), Times.Once);
    }

    [Fact]
    public async Task DeleteAporteMeta_ReturnsBadRequest_WhenServiceFails()
    {
        var aporteId = Guid.NewGuid();
        var metaId = Guid.NewGuid();
        var metaDto = new MetaDTO { MetaId = metaId, Titulo = "Test tittle", Descricao = "Test description", DataAlvo = DateTime.UtcNow.AddDays(4), DataInicio = DateTime.Now, PorcentagemAtual = 0, Status = Status.EmAberto, ValorAlvo = 10000 };
        var aporteDto = new AporteMetasDTO
        {
            MetaId = metaId,
            Valor = 350
        };
        _helper.serviceMock
            .Setup(s => s.RemoverAporteAsync(aporteId))
            .ReturnsAsync(ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta));

        var result = await _helper.controller.DeleteAporte(aporteId);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.RemoverAporteAsync(aporteId), Times.Once);
    }

}
