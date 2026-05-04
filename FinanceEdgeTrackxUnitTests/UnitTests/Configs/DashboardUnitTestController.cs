using FinanceEdgeTrack.Controllers;
using FinanceEdgeTrack.Domain.Interfaces.Services.Dashboard;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Configs;

public class DashboardUnitTestController
{
    public Mock<IDashboardService> dashboardServiceMock;
    public DashboardController controller;

    public DashboardUnitTestController()
    {
        dashboardServiceMock = new Mock<IDashboardService>();
        var serviceMock = dashboardServiceMock.Object;

        controller = new DashboardController(serviceMock);
    }

}
