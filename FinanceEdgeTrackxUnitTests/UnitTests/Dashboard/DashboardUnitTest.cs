using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Dashboard.Consolidado;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Dashboard;

public class DashboardUnitTest
{
    private readonly DashboardUnitTestController _helper;

    public DashboardUnitTest()
    {
        _helper = new DashboardUnitTestController();
    }

    private DashboardConsolidadoNoMesDTO CreateSeedMonth()
    {
        return new DashboardConsolidadoNoMesDTO
        {
            Saldo = 5400,
            MediaGastosDiarioNoMes = 32,
            ValorTotalGastoNoMes = 5500,
            ValorTotalRecebidoNoMes = 8000,
            ValorAlvoEmMetasNoMes = 120000,
            ValorAportadoEmMetasNoMes = 5000,
            MediaDiasParaConclusao = 15,
            TotalConcluidasNoMes = 1,
            TotalPendentesNoMes = 4,
            TotalCanceladasNoMes = 1,
        };
    }

    private DashboardConsolidadoPeriodoDTO CreateSeedPeriod()
    {
        return new DashboardConsolidadoPeriodoDTO
        {
            Saldo = 5400,
            MediaGastosDiarioNoPeriodo = 15,
            ValorTotalGastoNoPeriodo = 2500,
            ValorTotalRecebidoNoPeriodo = 4325,
            ValorAlvoEmMetasNoPeriodo = 8000,
            ValorAportadoEmMetasNoPeriodo = 300,
            TotalConcluidasNoPeriodo = 3,
            TotalPendentesNoPeriodo = 2,
            TotalCanceladasNoPeriodo = 4
        };
    }

    private DashboardConsolidadoDTO CreateSeedGeneral()
    {
        return new DashboardConsolidadoDTO
        {
            Saldo = 5400,
            ValorTotalGasto = 12000,
            ValorTotalGastosFixos = 7000,
            ValorTotalRecebido = 18000,
            ValorAlvoEmMetas = 68000,
            ValorAportadoEmMetas = 15000,
            PorcentagemConcluida = 24,
            ValorRestanteParaCompletarTodas = 43000,
            TotalConcluidas = 10,
            TotalPendentes = 20,
            TotalCanceladas = 3,
        };
    }


    [Fact]
    public async Task GetDashboardMonth_ReturnsOk_WhenCredentialsAreValid()
    {
        // arrange
        const int currentYear = 2026;
        const int month = 02;
        var dashboardDTO = CreateSeedMonth();
        var dashboard = _helper.dashboardServiceMock
                               .Setup(s => s.GetDashboardMensal(currentYear, month))
                               .ReturnsAsync(ApiResponse<DashboardConsolidadoNoMesDTO>.Ok(dashboardDTO));

        // act
        var result = await _helper.controller.GetDashboardMonth(currentYear, month);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<DashboardConsolidadoNoMesDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.True(currentYear >= 2026);
        _helper.dashboardServiceMock.Verify(s => s.GetDashboardMensal(currentYear, month), Times.Once);
    }


    [Fact]
    public async Task GetDashboardMonth_ReturnsBadRequest_WhenCredentiaslsAreInvalid()
    {
        // arrange
        const int currentYear = 2020;
        const int month = 02;
        var dashboardDTO = CreateSeedMonth();
        var dashboard = _helper.dashboardServiceMock
                               .Setup(s => s.GetDashboardMensal(currentYear, month))
                               .ReturnsAsync(ApiResponse<DashboardConsolidadoNoMesDTO>.Fail(ResultMessages.InvalidDateDashboard));

        // act
        var result = await _helper.controller.GetDashboardMonth(currentYear, month);

        // assert
        Assert.IsType<BadRequestResult>(result);
        Assert.False(currentYear >= 2026);
        _helper.dashboardServiceMock.Verify(s => s.GetDashboardMensal(currentYear, month), Times.Once);
    }

    [Fact]
    public async Task GetDashboardMonth_ReturnsBadRequest_WhenMetricsAreNull()
    {
        // arrange
        const int currentYear = 2026;
        const int month = 02;
        var dashboard = _helper.dashboardServiceMock
                               .Setup(s => s.GetDashboardMensal(currentYear, month))
                               .ReturnsAsync(ApiResponse<DashboardConsolidadoNoMesDTO>.Fail(ResultMessages.ErrorToCreateDashboard));

        // act
        var result = await _helper.controller.GetDashboardMonth(currentYear, month);

        // assert
        Assert.IsType<BadRequestResult>(result);
        Assert.True(currentYear >= 2026);
        _helper.dashboardServiceMock.Verify(s => s.GetDashboardMensal(currentYear, month), Times.Once);
    }

    [Fact]
    public async Task GetDashboardGeneral_ReturnsOk_WhenPeriodIsValidAndServiceReturnsSuccess()
    {
        // arrange
        var dashboardDTO = CreateSeedGeneral();
        var dashboard = _helper.dashboardServiceMock
                               .Setup(s => s.GetDashboardGeral())
                               .ReturnsAsync(ApiResponse<DashboardConsolidadoDTO>.Ok(dashboardDTO));

        // act
        var result = await _helper.controller.GetDashboardGeneral();

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<DashboardConsolidadoDTO>>(ok.Value);
        Assert.True(response.Success);
        _helper.dashboardServiceMock.Verify(s => s.GetDashboardGeral(), Times.Once);
    }

    [Fact]
    public async Task GetDashboardGeneral_ReturnsBadRequest_WhenServiceFails()
    {
        // arrange
        var dashboard = _helper.dashboardServiceMock
                               .Setup(s => s.GetDashboardGeral())
                               .ReturnsAsync(ApiResponse<DashboardConsolidadoDTO>.Fail(ResultMessages.ErrorToCreateDashboard));

        // act
        var result = await _helper.controller.GetDashboardGeneral();

        // assert
        Assert.IsType<BadRequestResult>(result);
        _helper.dashboardServiceMock.Verify(s => s.GetDashboardGeral(), Times.Once);
    }

    [Fact]
    public async Task GetDashboardPeriod_ReturnsOk_WhenServiceReturnsSuccess()
    {
        // arrange
        DateTime start = new DateTime(2026, 02, 20);
        DateTime end = new DateTime(2026, 05, 20);
        var dashboardDTO = CreateSeedPeriod();
        var dashboard = _helper.dashboardServiceMock
                               .Setup(s => s.GetDashboardPeriodo(start, end))
                               .ReturnsAsync(ApiResponse<DashboardConsolidadoPeriodoDTO>.Ok(dashboardDTO));

        // act
        var result = await _helper.controller.GetDashboardPeriod(start, end);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<DashboardConsolidadoPeriodoDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.True(start <= end);
        Assert.True(start.Year == 2026);
        _helper.dashboardServiceMock.Verify(s => s.GetDashboardPeriodo(start, end), Times.Once);
    }

    [Fact]
    public async Task GetDashboardPeriod_ReturnsBadRequest_WhenPeriodIsInvalid()
    {
        // arrange
        DateTime start = new DateTime(2026, 02, 20);
        DateTime end = new DateTime(2026, 02, 15);
        var dashboard = _helper.dashboardServiceMock
                               .Setup(s => s.GetDashboardPeriodo(start, end))
                               .ReturnsAsync(ApiResponse<DashboardConsolidadoPeriodoDTO>.Fail(ResultMessages.ErrorToCreateDashboard));

        // act
        var result = await _helper.controller.GetDashboardPeriod(start, end);

        // assert
        Assert.IsType<BadRequestResult>(result);
        Assert.True(start >= end);
        _helper.dashboardServiceMock.Verify(s => s.GetDashboardPeriodo(start, end), Times.Once);
    }

    [Fact]
    public async Task GetDashboardPeriod_ReturnsBadRequest_WhenServiceFails()
    {
        // arrange
        DateTime start = new DateTime(2026, 02, 20);
        DateTime end = new DateTime(2026, 05, 28);
        var dashboard = _helper.dashboardServiceMock
                               .Setup(s => s.GetDashboardPeriodo(start, end))
                               .ReturnsAsync(ApiResponse<DashboardConsolidadoPeriodoDTO>.Fail(ResultMessages.ErrorToCreateDashboard));

        // act
        var result = await _helper.controller.GetDashboardPeriod(start, end);

        // assert
        Assert.IsType<BadRequestResult>(result);
        _helper.dashboardServiceMock.Verify(s => s.GetDashboardPeriodo(start, end), Times.Once);
    }


}
