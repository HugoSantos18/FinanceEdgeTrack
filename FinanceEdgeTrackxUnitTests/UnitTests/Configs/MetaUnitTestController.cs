using FinanceEdgeTrack.Controllers;
using FinanceEdgeTrack.Domain.Interfaces.Services.Categories;
using MapsterMapper;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Configs;

public class MetaUnitTestController
{
    public MetaController controller;
    public IMapper mapper;
    public Mock<IMetaService> serviceMock;

    public MetaUnitTestController()
    {
        var config = Mapster.TypeAdapterConfig.GlobalSettings;
        mapper = new Mapper(config);

        serviceMock = new Mock<IMetaService>();
        var service = serviceMock.Object;

        controller = new MetaController(service);
    }

}
