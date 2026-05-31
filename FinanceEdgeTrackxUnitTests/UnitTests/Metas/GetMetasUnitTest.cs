using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read.Metas;
using FinanceEdgeTrack.Domain.Enums;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Metas;

public class GetMetasUnitTest
{
    private readonly MetaUnitTestController _helper;

    public GetMetasUnitTest()
    {
        _helper = new MetaUnitTestController();
    }

    private PagedList<MetaDTO> CreateSeedAndPaginate()
    {
        var items = new List<MetaDTO>
        {
            new() {MetaId = Guid.NewGuid(), Titulo = "Meta1", Descricao = "Meta1", DataAlvo = DateTime.UtcNow.AddDays(20), DataInicio = DateTime.UtcNow, ValorAlvo = 1000, PorcentagemAtual = 0, Status = Status.Concluido},
            new() {MetaId = Guid.NewGuid(), Titulo = "Meta2", Descricao = "Meta2", DataAlvo = DateTime.UtcNow.AddDays(10), DataInicio = DateTime.UtcNow, ValorAlvo = 3000, PorcentagemAtual = 0, Status = Status.EmAberto},
            new() {MetaId = Guid.NewGuid(), Titulo = "Meta3", Descricao = "Meta3", DataAlvo = DateTime.UtcNow.AddDays(30), DataInicio = DateTime.UtcNow, ValorAlvo = 4000, PorcentagemAtual = 0, Status = Status.EmAberto},
        };

        var aportesMetas = new List<AporteMetasDTO>
        {
            new() {MetaId = items[1].MetaId, Valor = 150},
            new() {MetaId = items[1].MetaId, Valor = 200},
            new() {MetaId = items[2].MetaId, Valor = 100},
            new() {MetaId = items[2].MetaId, Valor = 250}
        };

        var paged = new PagedList<MetaDTO>(items, 3, 1, 10);
        return paged;
    }


    [Fact]
    public async Task GetMetaAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var id = Guid.NewGuid();
        var metaDto = new MetaDTO
        {
            MetaId = id,
            Titulo = "Meta teste",
            Descricao = "Descrição teste",
            DataAlvo = DateTime.UtcNow.AddDays(20),
            DataInicio = DateTime.UtcNow,
            PorcentagemAtual = 0,
            Status = Status.EmAberto,
            ValorAlvo = 3000,
        };
        _helper.serviceMock
            .Setup(s => s.GetMetaPorIdAsync(id))
            .ReturnsAsync(ApiResponse<MetaDTO>.Ok(metaDto));


        var result = await _helper.controller.GetMetaAsync(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(id, response.Data!.MetaId);
        _helper.serviceMock.Verify(s => s.GetMetaPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetMetaAsync_ReturnsNotFound_WhenServiceFails()
    {
        var id = Guid.NewGuid();
        _helper.serviceMock
           .Setup(s => s.GetMetaPorIdAsync(id))
           .ReturnsAsync(ApiResponse<MetaDTO>.Fail(ResultMessages.NotFoundMeta));


        var result = await _helper.controller.GetMetaAsync(id);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<ApiResponse<MetaDTO>>(notFound.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.GetMetaPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasAsync_ReturnsOk_WhenServiceRetursSuccess()
    {
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.GetAllMetasAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllMetasAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.Count, response.Data!.Count);
        _helper.serviceMock.Verify(s => s.GetAllMetasAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
            .Setup(s => s.GetAllMetasAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Fail("Sem metas ainda."));

        var result = await _helper.controller.GetAllMetasAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.GetAllMetasAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByMostValueAsync_ReturnsOk_WhenServiceRetursSuccess()
    {
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.MetasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllMetasFilterByMostValueAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.Count, response.Data!.Count);
        _helper.serviceMock.Verify(s => s.MetasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByMostValueAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
            .Setup(s => s.MetasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Fail("Sem metas ainda."));

        var result = await _helper.controller.GetAllMetasFilterByMostValueAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.MetasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByLesserValueAsync_ReturnsOk_WhenServiceRetursSuccess()
    {
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.MetasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllMetasFilterByLesserValueAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.Count, response.Data!.Count);
        _helper.serviceMock.Verify(s => s.MetasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByLesserValueAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
            .Setup(s => s.MetasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Fail("Sem metas ainda."));

        var result = await _helper.controller.GetAllMetasFilterByLesserValueAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.MetasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByAlmostDoneAsync_ReturnsOk_WhenServiceRetursSuccess()
    {
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.MetasFiltradasQuaseConcluidasAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllMetasFilterByAlmostDoneAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.Count, response.Data!.Count);
        _helper.serviceMock.Verify(s => s.MetasFiltradasQuaseConcluidasAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByAlmostValueAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
           .Setup(s => s.MetasFiltradasQuaseConcluidasAsync(It.IsAny<PaginationParams>()))
           .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Fail("Sem metas ainda."));

        var result = await _helper.controller.GetAllMetasFilterByAlmostDoneAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.MetasFiltradasQuaseConcluidasAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByOldestAsync_ReturnsOk_WhenServiceRetursSuccess()
    {
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.MetasFiltradasMaisAntigaAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllMetasFilterByOldestAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.Count, response.Data!.Count);
        _helper.serviceMock.Verify(s => s.MetasFiltradasMaisAntigaAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByOldestAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
           .Setup(s => s.MetasFiltradasMaisAntigaAsync(It.IsAny<PaginationParams>()))
           .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Fail("Sem metas ainda."));

        var result = await _helper.controller.GetAllMetasFilterByOldestAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.MetasFiltradasMaisAntigaAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByRecentlyAsync_ReturnsOk_WhenServiceRetursSuccess()
    {
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.MetasFiltradasMaisRecentesAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllMetasFilterByRecentlyAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.Count, response.Data!.Count);
        _helper.serviceMock.Verify(s => s.MetasFiltradasMaisRecentesAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByRecentlyAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
           .Setup(s => s.MetasFiltradasMaisRecentesAsync(It.IsAny<PaginationParams>()))
           .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Fail("Sem metas ainda."));

        var result = await _helper.controller.GetAllMetasFilterByRecentlyAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.MetasFiltradasMaisRecentesAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByStatusAsync_ReturnsOk_WhenServiceRetursSuccess()
    {
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.MetasFiltradasPorStatusAsync(It.IsAny<StatusParams>()))
            .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllMetasFilterByStatusAsync(new StatusParams { Status = Status.EmAberto, PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.Count, response.Data!.Count);
        _helper.serviceMock.Verify(s => s.MetasFiltradasPorStatusAsync(It.IsAny<StatusParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllMetasFilterByStatusAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
           .Setup(s => s.MetasFiltradasPorStatusAsync(It.IsAny<StatusParams>()))
           .ReturnsAsync(ApiResponse<PagedList<MetaDTO>>.Fail("Sem metas ainda."));

        var result = await _helper.controller.GetAllMetasFilterByStatusAsync(new StatusParams { Status = Status.EmAberto, PageNumber = 1, PageSize = 10});

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<MetaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.MetasFiltradasPorStatusAsync(It.IsAny<StatusParams>()), Times.Once);
    }
}
