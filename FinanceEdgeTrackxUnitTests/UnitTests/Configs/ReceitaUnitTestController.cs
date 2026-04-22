using FinanceEdgeTrack.Controllers;
using FinanceEdgeTrack.Domain.Interfaces.Services.Categories;
using MapsterMapper;
using Moq;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Configs;

public class ReceitaUnitTestController
{
    public ReceitaController controller;
    public IMapper mapper;
    public Mock<IReceitaService> serviceMock;

    public ReceitaUnitTestController()
    {
        serviceMock = new Mock<IReceitaService>();
        var service = serviceMock.Object;  

        controller = new ReceitaController(service);

        var config = Mapster.TypeAdapterConfig.GlobalSettings;
        mapper = new Mapper(config);
    }
}
