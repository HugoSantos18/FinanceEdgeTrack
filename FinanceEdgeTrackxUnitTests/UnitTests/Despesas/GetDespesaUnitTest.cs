using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Categorias;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Despesas;


public class GetDespesaUnitTest
{
    private readonly DespesaUnitTestController _helper;

    public GetDespesaUnitTest()
    {
        _helper = new DespesaUnitTestController();
    }

    private PagedList<DespesaDTO> CreateSeedAndPaginate()
    {
        var items = new List<DespesaDTO>
    {
        new() { DespesaId = Guid.NewGuid(), Titulo = "Despesa1", Valor = 150, Fixa = false },
        new() { DespesaId = Guid.NewGuid(), Titulo = "Despesa2", Valor = 300, Fixa = true },
        new() { DespesaId = Guid.NewGuid(), Titulo = "Despesa3", Valor = 220, Fixa = true }
    };

        var paged = new PagedList<DespesaDTO>(items, 3, 1, 10);
        return paged;
    }

    [Fact]
    public async Task GetAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        // arrange
        Guid id = Guid.NewGuid();
        var despesaDTO = new DespesaDTO
        {
            DespesaId = id,
            Titulo = "Teste",
            Valor = 100,
            Fixa = false,
            Data = DateTime.UtcNow,
            Descricao = "Descrição teste"
        };

        _helper.serviceMock
            .Setup(s => s.ObterDespesaPorIdAsync(id))
            .ReturnsAsync(ApiResponse<DespesaDTO>.Ok(despesaDTO));

        // act
        var resultRequest = await _helper.controller.GetAsync(id);

        // assert
        var ok = Assert.IsType<OkObjectResult>(resultRequest);
        var response = Assert.IsType<ApiResponse<DespesaDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(id, response.Data!.DespesaId);
        _helper.serviceMock.Verify(s => s.ObterDespesaPorIdAsync(id), Times.Once);
    }


    [Fact]
    public async Task GetAsync_ReturnsNotFound_WhenServiceFails()
    {
        // arrange
        var id = Guid.NewGuid();
        _helper.serviceMock
               .Setup(s => s.ObterDespesaPorIdAsync(id))
               .ReturnsAsync(ApiResponse<DespesaDTO>.Fail(ResultMessages.NotFoundDespesa));

        // act
        var resultRequest = await _helper.controller.GetAsync(id);


        // assert
        var notFound = Assert.IsType<NotFoundObjectResult>(resultRequest);
        var response = Assert.IsType<ApiResponse<DespesaDTO>>(notFound.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.ObterDespesaPorIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        // arrange
        var paged = CreateSeedAndPaginate();

        _helper.serviceMock
            .Setup(s => s.ListarDespesasAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<DespesaDTO>>.Ok(paged));

        // act
        var resultRequest = await _helper.controller.GetAllAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        // assert
        var ok = Assert.IsType<OkObjectResult>(resultRequest);
        var response = Assert.IsType<ApiResponse<PagedList<DespesaDTO>>>(ok.Value);
        Assert.True(response.Success);
        _helper.serviceMock.Verify(s => s.ListarDespesasAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsBadRequest_WhenServiceFails()
    {
        // arrange
        _helper.serviceMock
               .Setup(s => s.ListarDespesasAsync(It.IsAny<PaginationParams>()))
               .ReturnsAsync(ApiResponse<PagedList<DespesaDTO>>.Fail("Sem depesas ainda."));

        // act
        var resultRequest = await _helper.controller.GetAllAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        // assert
        var bad = Assert.IsType<BadRequestObjectResult>(resultRequest);
        var response = Assert.IsType<ApiResponse<PagedList<DespesaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.ListarDespesasAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllFixasAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
               .Setup(s => s.DespesasFixasPaginadasAsync(It.IsAny<PaginationParams>()))
               .ReturnsAsync(ApiResponse<PagedList<DespesaDTO>>.Ok(paged));

        var resultRequest = await _helper.controller.GetAllFixasAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(resultRequest);
        var response = Assert.IsType<ApiResponse<PagedList<DespesaDTO>>>(ok.Value);
        Assert.True(response.Success);
        _helper.serviceMock.Verify(s => s.DespesasFixasPaginadasAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllFixasAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
            .Setup(s => s.DespesasFixasPaginadasAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<DespesaDTO>>.Fail("Sem despesas ainda."));

        var resultRequestRequest = await _helper.controller.GetAllFixasAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });


        var bad = Assert.IsType<BadRequestObjectResult>(resultRequestRequest);
        var response = Assert.IsType<ApiResponse<PagedList<DespesaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.DespesasFixasPaginadasAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllByMostValueDescendingAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        // arrange
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.DespesasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<DespesaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllFilterByMostValueDescendingAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<DespesaDTO>>>(ok.Value);
        Assert.True(response.Success);
        _helper.serviceMock.Verify(s => s.DespesasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllByMostValueDescendingAsync_ReturnsBadRequest_WhenServiceFails()
    {
        _helper.serviceMock
            .Setup(s => s.DespesasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<DespesaDTO>>.Fail("Sem despesas ainda."));

        var result = await _helper.controller.GetAllFilterByMostValueDescendingAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<DespesaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.DespesasFiltradasMaiorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllByLessValueAscendingAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        // arrange
        var paged = CreateSeedAndPaginate();
        _helper.serviceMock
            .Setup(s => s.DespesasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<DespesaDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllFilterByLessValueAscendingAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<DespesaDTO>>>(ok.Value);
        Assert.True(response.Success);
        _helper.serviceMock.Verify(s => s.DespesasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAllByLessValueAscendingAsync_ReturnsBadRequest_WhenServiceFails()
    {
        // arrange
        _helper.serviceMock
            .Setup(s => s.DespesasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<DespesaDTO>>.Fail(ResultMessages.EmptyCollection));

        // act
        var resultRequest = await _helper.controller.GetAllFilterByLessValueAscendingAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        // assert
        var bad = Assert.IsType<BadRequestObjectResult>(resultRequest);
        var response = Assert.IsType<ApiResponse<PagedList<DespesaDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.DespesasFiltradasMenorValorAsync(It.IsAny<PaginationParams>()), Times.Once);
    }
}