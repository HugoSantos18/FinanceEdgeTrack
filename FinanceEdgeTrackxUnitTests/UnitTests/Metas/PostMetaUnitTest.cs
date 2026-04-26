using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Application.Dtos.Write.Categorias;
using FinanceEdgeTrack.Domain.Enum;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Metas;

public class PostMetaUnitTest
{
    private readonly MetaUnitTestController _helper;

    public PostMetaUnitTest()
    {
        _helper = new MetaUnitTestController();
    }

    private CreateMetaDTO CreateSeedMetas(int index)
    {
        var items = new List<CreateMetaDTO>
        {
            new() { Titulo = "Meta1", Descricao = "Meta1", DataAlvo = DateTime.UtcNow.AddDays(20), DataInicio = DateTime.UtcNow, ValorAlvo = 1000, Aportes = new() },
            new() { Titulo = "Meta2", Descricao = "Meta2", DataAlvo = DateTime.UtcNow.AddDays(10), DataInicio = DateTime.UtcNow, ValorAlvo = -1000, Aportes = new() },
            new() { Titulo = "Meta3", Descricao = "Meta3", DataAlvo = DateTime.UtcNow.AddDays(3), DataInicio = DateTime.UtcNow, ValorAlvo = 4000 , Aportes = new()},
            new() { Titulo = "", Descricao = "Meta4", DataAlvo = DateTime.UtcNow.AddDays(5), DataInicio = DateTime.UtcNow, ValorAlvo = 1800, Aportes = new() },
        };

        return items[index];
    }

    [Fact]
    public async Task PostMeta_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var createMetaDto = CreateSeedMetas(0);
        _helper.serviceMock
            .Setup(s => s.CriarMetaAsync(createMetaDto))
            .ReturnsAsync(ApiResponse<MetaDTO>.Ok(_helper.mapper.Map<MetaDTO>(createMetaDto)));

        var result = await _helper.controller.Post(createMetaDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.True(response.Data!.ValorAlvo > 0);
        Assert.Equal(createMetaDto.ValorAlvo, response.Data!.ValorAlvo);
        Assert.Equal(createMetaDto.Titulo, response.Data!.Titulo);
        _helper.serviceMock.Verify(s => s.CriarMetaAsync(createMetaDto), Times.Once);
    }

    [Fact]
    public async Task PostInvalidValueMeta_ReturnsBadRequest_WhenServiceFails()
    {
        var createMetaDto = CreateSeedMetas(1);
        _helper.serviceMock
            .Setup(s => s.CriarMetaAsync(createMetaDto))
            .ReturnsAsync(ApiResponse<MetaDTO>.Fail(ResultMessages.InvalidCredentials));

        var result = await _helper.controller.Post(createMetaDto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.CriarMetaAsync(createMetaDto), Times.Once);
    }

    [Fact]
    public async Task PostAportesInMeta_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var meta = new MetaDTO
        {
            MetaId = Guid.NewGuid(),
            Titulo = "Meta Teste",
            Descricao = "Descricao meta teste",
            DataInicio = DateTime.UtcNow,
            DataAlvo = DateTime.UtcNow.AddDays(20),
            PorcentagemAtual = 0,
            Status = Status.EmAberto,
            ValorAlvo = 1200,
        };
        var aporteDto = new CreateAporteMetaDTO
        {
            Valor = 200
        };

        _helper.serviceMock
            .Setup(s => s.RegistrarAporteAsync(meta.MetaId, aporteDto))
            .ReturnsAsync(ApiResponse<MetaDTO>.Ok(meta));

        var result = await _helper.controller.PostAporte(meta.MetaId, aporteDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.True(response.Data!.ValorAlvo > 0);
        Assert.True(response.Data!.Status == Status.EmAberto); 
        Assert.False(response.Data!.Titulo.IsNullOrEmpty());
        Assert.Equal(meta.MetaId, response.Data!.MetaId);
        Assert.Equal(meta.ValorAlvo, response.Data!.ValorAlvo);
        _helper.serviceMock.Verify(s => s.RegistrarAporteAsync(meta.MetaId, aporteDto), Times.AtMost(3));
    }

    [Fact]
    public async Task PostInvalidAportesInMeta_ReturnsBadRequest_WhenServiceFails()
    {
        var metaId = Guid.NewGuid();
        var aporteDto = new CreateAporteMetaDTO
        {
            Valor = 200
        };

        _helper.serviceMock
            .Setup(s => s.RegistrarAporteAsync(metaId, aporteDto))
            .ReturnsAsync(ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta));

        var result = await _helper.controller.PostAporte(metaId, aporteDto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.RegistrarAporteAsync(metaId, aporteDto), Times.AtMost(3));
    }

    [Fact]
    public async Task PostInvalidDataMeta_ReturnsBadRequest_WhenServiceFails()
    {
        var createMetaDto = CreateSeedMetas(2);
        _helper.serviceMock
            .Setup(s => s.CriarMetaAsync(createMetaDto))
            .ReturnsAsync(ApiResponse<MetaDTO>.Fail(ResultMessages.InvalidCredentials));

        var result = await _helper.controller.Post(createMetaDto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.CriarMetaAsync(createMetaDto), Times.Once);
    }
}
