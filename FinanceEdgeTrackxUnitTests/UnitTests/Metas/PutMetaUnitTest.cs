using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using FinanceEdgeTrack.Domain.Enum;
using Moq;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using Microsoft.AspNetCore.Mvc;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Metas;

public class PutMetaUnitTest
{
    private readonly MetaUnitTestController _helper;

    public PutMetaUnitTest()
    {
        _helper = new MetaUnitTestController();
    }

    [Fact]
    public async Task PutMeta_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var id = Guid.NewGuid();
        var updateMetaDto = new UpdateMetaDTO
        {
            MetaId = id,
            Titulo = "Test tittle",
            DataAlvo = DateTime.UtcNow.AddDays(10),
            Status = Status.EmAberto,
            ValorAlvo = 5000
        };
        var mapped = _helper.mapper.Map<MetaDTO>(updateMetaDto);
        _helper.serviceMock
            .Setup(s => s.AtualizarMetaAsync(id, It.IsAny<UpdateMetaDTO>()))
            .ReturnsAsync(ApiResponse<MetaDTO>.Ok(mapped));


        var result = await _helper.controller.Put(id, updateMetaDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(mapped.MetaId, response.Data!.MetaId);
        _helper.serviceMock.Verify(s => s.AtualizarMetaAsync(id, It.IsAny<UpdateMetaDTO>()), Times.Once);
    }

    [Fact]
    public async Task PutMeta_ReturnsBadRequest_WhenServiceFails()
    {
        var id = Guid.NewGuid();
        var updateMetaDto = new UpdateMetaDTO
        {
            MetaId = id,
            Titulo = "Test tittle",
            DataAlvo = DateTime.UtcNow.AddDays(10),
            Status = Status.EmAberto,
            ValorAlvo = 5000
        };
        _helper.serviceMock
            .Setup(s => s.AtualizarMetaAsync(id, It.IsAny<UpdateMetaDTO>()))
            .ReturnsAsync(ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta));


        var result = await _helper.controller.Put(id, updateMetaDto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.AtualizarMetaAsync(id, It.IsAny<UpdateMetaDTO>()), Times.Once);
    }

}
