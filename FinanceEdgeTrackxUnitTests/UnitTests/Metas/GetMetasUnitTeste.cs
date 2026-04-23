using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Pagination.Filters;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrack.Domain.Enum;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Metas;

public class GetMetasUnitTeste
{
    private readonly MetaUnitTestController _helper;

    public GetMetasUnitTeste()
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
    public async Task GetAporteAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        // arrange
        var aporteId = Guid.NewGuid();
        var aporteDto = new AporteMetasDTO
        {
            MetaId = Guid.NewGuid(),
            Valor = 150
        };

        _helper.serviceMock
            .Setup(s => s.GetAportePorIdAsync(aporteId))
            .ReturnsAsync(ApiResponse<AporteMetasDTO>.Ok(aporteDto));

        // act
        var result = await _helper.controller.GetAporteAsync(aporteId);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<AporteMetasDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(aporteDto.Valor, response.Data!.Valor);
        _helper.serviceMock.Verify(s => s.GetAportePorIdAsync(aporteId), Times.Once);
    }

    [Fact]
    public async Task GetAporteAsync_ReturnsNotFound_WhenServiceFails()
    {
        // arrange
        var metaId = Guid.NewGuid();
        _helper.serviceMock
            .Setup(s => s.GetAllAportesDaMetaPorIdAsync(metaId, It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<AporteMetasDTO>>.Fail("Sem aportes ainda."));

        // act
        var result = await _helper.controller.GetAllAportesAsync(metaId, new PaginationParams { PageNumber = 1, PageSize = 10 });

        // assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<AporteMetasDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.GetAllAportesDaMetaPorIdAsync(metaId, It.IsAny<PaginationParams>()), Times.Once);
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
    public async Task GetAllAportesAsync_ReturnsOk_WhenServiceRetursSuccess()
    {
        // arrange
        var metaId = Guid.NewGuid();
        var aporte1 = new AporteMetasDTO { MetaId = metaId, Valor = 150 };
        var aporte2 = new AporteMetasDTO { MetaId = metaId, Valor = 200 };
        var items = new List<AporteMetasDTO> { aporte1, aporte2 };

        // create PagedList<AporteMetasDTO> via non-public constructor
        var pagedListType = typeof(PagedList<AporteMetasDTO>);
        var ctor = pagedListType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new Type[] { typeof(IEnumerable<AporteMetasDTO>), typeof(int), typeof(int), typeof(int) },
            null);

        if (ctor == null)
            throw new InvalidOperationException("PagedList constructor not found");

        var paged = (PagedList<AporteMetasDTO>)ctor.Invoke(new object[] { items, items.Count, 1, 10 });

        _helper.serviceMock
            .Setup(s => s.GetAllAportesDaMetaPorIdAsync(metaId, It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<AporteMetasDTO>>.Ok(paged));

        // act
        var result = await _helper.controller.GetAllAportesAsync(metaId, new PaginationParams { PageNumber = 1, PageSize = 10 });

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<AporteMetasDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.TotalCount, response.Data!.TotalCount);
        Assert.Equal(paged.Count, response.Data.Count);
        _helper.serviceMock.Verify(s => s.GetAllAportesDaMetaPorIdAsync(metaId, It.IsAny<PaginationParams>()), Times.Once);

    }

    [Fact]
    public async Task GetAllAportesAsync_ReturnsBadRequest_WhenServiceFails()
    {
        // arrange
        var metaId = Guid.NewGuid();    
        var aporte = new AporteMetasDTO { MetaId = Guid.NewGuid(), Valor = 150 };

        _helper.serviceMock
            .Setup(s => s.GetAllAportesDaMetaPorIdAsync(aporte.MetaId, It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<AporteMetasDTO>>.Fail("Sem aportes ainda."));

        // act
        var result = await _helper.controller.GetAllAportesAsync(aporte.MetaId, new PaginationParams { PageNumber = 1, PageSize = 10 });

        // assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<AporteMetasDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.GetAllAportesDaMetaPorIdAsync(aporte.MetaId, It.IsAny<PaginationParams>()), Times.Once);
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
