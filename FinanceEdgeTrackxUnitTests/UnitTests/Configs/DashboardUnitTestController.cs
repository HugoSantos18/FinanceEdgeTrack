using FinanceEdgeTrack.Domain.Interfaces.Services.Dashboard;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Configs;

public class DashboardUnitTestController
{
    public Mock<IDashboardService> dashboardServiceMock;

    public DashboardUnitTestController()
    {
        dashboardServiceMock = new Mock<IDashboardService>();
        var serviceMock = dashboardServiceMock.Object;
    }

}
