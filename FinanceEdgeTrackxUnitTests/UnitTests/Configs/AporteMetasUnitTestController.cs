using FinanceEdgeTrack.Controllers;
using FinanceEdgeTrack.Application.Interfaces.Services.Categories;
using MapsterMapper;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Configs;

public class AporteMetasUnitTestController
{
    public AporteMetasController controller;
    public IMapper mapper;
    public Mock<IAporteMetasService> serviceMock;

    public AporteMetasUnitTestController()
    {
        var config = Mapster.TypeAdapterConfig.GlobalSettings;
        mapper = new Mapper(config);

        serviceMock = new Mock<IAporteMetasService>();
        var service = serviceMock.Object;

        controller = new AporteMetasController(service);
    }
}
