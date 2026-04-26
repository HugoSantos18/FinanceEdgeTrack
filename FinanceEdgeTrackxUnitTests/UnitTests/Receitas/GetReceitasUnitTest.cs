using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Receitas;

public class GetReceitasUnitTest
{
    private readonly ReceitaUnitTestController _helper;

    public GetReceitasUnitTest()
    {
        _helper = new ReceitaUnitTestController();
    }

    private PagedList<ReceitaDTO> CreateSeedAndPaginate()
    {
        var items = new List<ReceitaDTO>
        {
            new() {ReceitaId = Guid.NewGuid(), Titulo = "Receita1", Descricao = "Descrição 1", Valor = 200, Data = DateTime.UtcNow },
            new() {ReceitaId = Guid.NewGuid(), Titulo = "Receita2", Descricao = "Descrição 2", Valor = 100, Data = DateTime.UtcNow },
            new() {ReceitaId = Guid.NewGuid(), Titulo = "Receita3", Descricao = "Descrição 3", Valor = 180, Data = DateTime.UtcNow }
        };

        var paged = new PagedList<ReceitaDTO>(items, 3, 1, 10);
        return paged;
    }

    [Fact]
    public async Task GetAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var id = Guid.NewGuid();
        var receitaDto = new ReceitaDTO
        {
            ReceitaId = id,
            Titulo = "Receita teste",
            Descricao = "Descrição teste",
            Valor = 300,
            Data = DateTime.UtcNow
        };
        _helper.serviceMock
            .Setup(s => s.ObterReceitaPorIdAsync(id))
            .ReturnsAsync(ApiResponse<ReceitaDTO>.Ok(receitaDto));

        var result = await _helper.controller.GetAsync(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ReceitaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(id, response.Data!.ReceitaId);
        _helper.serviceMock.Verify(s => s.ObterReceitaPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ReturnsNotFound_WhenServiceFails()
    {
        var id = Guid.NewGuid();
        _helper.serviceMock
            .Setup(s => s.ObterReceitaPorIdAsync(id))
            .ReturnsAsync(ApiResponse<ReceitaDTO>.Fail(ResultMessages.NotFoundReceive));

        var result = await _helper.controller.GetAsync(id);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ReceitaDTO>>(notFound.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.ObterReceitaPorIdAsync(id), Times.Once);
    }


    [Fact]
    public async Task GetAllAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.ListarReceitasAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<ReceitaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<ReceitaDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.Count, response.Data!.Count);
        _helper.serviceMock.Verify(s => s.ListarReceitasAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
            .Setup(s => s.ListarReceitasAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<ReceitaDTO>>.Fail("Sem receitas."));

        var result = await _helper.controller.GetAllAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<ReceitaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.ListarReceitasAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllFilterByValueDescendingAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.ReceitasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<ReceitaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllFilterByValueDescendingAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<ReceitaDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.Count, response.Data!.Count);
        _helper.serviceMock.Verify(s => s.ReceitasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllFilterByValueDescendingAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
           .Setup(s => s.ReceitasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()))
           .ReturnsAsync(ApiResponse<PagedList<ReceitaDTO>>.Fail("Sem receitas."));

        var result = await _helper.controller.GetAllFilterByValueDescendingAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<ReceitaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.ReceitasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllFilterByValueAscendingAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.ReceitasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<ReceitaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllFilterByValueAscendingAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<ReceitaDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.Count, response.Data!.Count);
        _helper.serviceMock.Verify(s => s.ReceitasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllFilterByValueAscendingAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
           .Setup(s => s.ReceitasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()))
           .ReturnsAsync(ApiResponse<PagedList<ReceitaDTO>>.Fail("Sem receitas."));

        var result = await _helper.controller.GetAllFilterByValueAscendingAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<ReceitaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.ReceitasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }
}
