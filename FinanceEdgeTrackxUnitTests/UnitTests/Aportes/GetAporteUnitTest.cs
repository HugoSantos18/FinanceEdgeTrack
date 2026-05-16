using FinanceEdgeTrack.Application.Common.Pagination;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Metas;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Aportes;

public class GetAporteUnitTest
{
    private readonly AporteMetasUnitTestController _helper;

    public GetAporteUnitTest()
    {
        _helper = new AporteMetasUnitTestController();
    }

    [Fact]
    public async Task GetAporteById_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var aporteId = Guid.NewGuid();
        var aporteDto = new AporteMetasDTO { MetaId = Guid.NewGuid(), Valor = 150 };

        _helper.serviceMock
            .Setup(s => s.GetAporteByIdAsync(aporteId))
            .ReturnsAsync(ApiResponse<AporteMetasDTO>.Ok(aporteDto));

        var result = await _helper.controller.GetByIdAsync(aporteId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<AporteMetasDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(aporteDto.Valor, response.Data!.Valor);
        _helper.serviceMock.Verify(s => s.GetAporteByIdAsync(aporteId), Times.Once);
    }

    [Fact]
    public async Task GetAporteById_ReturnsNotFound_WhenServiceFails()
    {
        var aporteId = Guid.NewGuid();

        _helper.serviceMock
            .Setup(s => s.GetAporteByIdAsync(aporteId))
            .ReturnsAsync(ApiResponse<AporteMetasDTO>.Fail(ResultMessages.NotFoundAporte));

        var result = await _helper.controller.GetByIdAsync(aporteId);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<ApiResponse<AporteMetasDTO>>(notFound.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.GetAporteByIdAsync(aporteId), Times.Once);
    }

    [Fact]
    public async Task GetAportesDaMeta_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var metaId = Guid.NewGuid();
        var items = new List<AporteMetasDTO>
        {
            new() { MetaId = metaId, Valor = 150 },
            new() { MetaId = metaId, Valor = 200 }
        };

        var ctor = typeof(PagedList<AporteMetasDTO>).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new Type[] { typeof(IEnumerable<AporteMetasDTO>), typeof(int), typeof(int), typeof(int) },
            null);

        if (ctor == null)
            throw new InvalidOperationException("PagedList constructor not found");

        var paged = (PagedList<AporteMetasDTO>)ctor.Invoke(new object[] { items, items.Count, 1, 10 });

        _helper.serviceMock
            .Setup(s => s.GetAportesDaMetaAsync(metaId, It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<AporteMetasDTO>>.Ok(paged));

        var result = await _helper.controller.GetAllByMetaAsync(metaId, new PaginationParams { PageNumber = 1, PageSize = 10 });

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<AporteMetasDTO>>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(paged.TotalCount, response.Data!.TotalCount);
        Assert.Equal(paged.Count, response.Data.Count);
        _helper.serviceMock.Verify(s => s.GetAportesDaMetaAsync(metaId, It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetAportesDaMeta_ReturnsBadRequest_WhenServiceFails()
    {
        var metaId = Guid.NewGuid();

        _helper.serviceMock
            .Setup(s => s.GetAportesDaMetaAsync(metaId, It.IsAny<PaginationParams>()))
            .ReturnsAsync(ApiResponse<PagedList<AporteMetasDTO>>.Fail(ResultMessages.NotFoundMeta));

        var result = await _helper.controller.GetAllByMetaAsync(metaId, new PaginationParams { PageNumber = 1, PageSize = 10 });

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ApiResponse<PagedList<AporteMetasDTO>>>(bad.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.GetAportesDaMetaAsync(metaId, It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task GetTotalDaMeta_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var metaId = Guid.NewGuid();

        _helper.serviceMock
            .Setup(s => s.ValorTotalDaMetaAsync(metaId))
            .ReturnsAsync(ApiResponse<decimal>.Ok(750m, "Valor total investido na meta R$ 750,00"));

        var result = await _helper.controller.GetTotalDaMetaAsync(metaId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<decimal>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(750m, response.Data);
        _helper.serviceMock.Verify(s => s.ValorTotalDaMetaAsync(metaId), Times.Once);
    }

    [Fact]
    public async Task GetTotalDaMeta_ReturnsNotFound_WhenServiceFails()
    {
        var metaId = Guid.NewGuid();

        _helper.serviceMock
            .Setup(s => s.ValorTotalDaMetaAsync(metaId))
            .ReturnsAsync(ApiResponse<decimal>.Fail(ResultMessages.NotFoundMeta));

        var result = await _helper.controller.GetTotalDaMetaAsync(metaId);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<ApiResponse<decimal>>(notFound.Value);
        Assert.False(response.Success);
        _helper.serviceMock.Verify(s => s.ValorTotalDaMetaAsync(metaId), Times.Once);
    }
}
