using FinanceEdgeTrack.Domain.Interfaces.Services.Categories;
using Mapster;
using MapsterMapper;
using Moq;
using FinanceEdgeTrack.Controllers;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Configs;

public class DespesaUnitTestController
{
    public IMapper mapper;
    public DespesaController controller;
    public Mock<IDespesaService> serviceMock;

    public DespesaUnitTestController()
    {
        // configurar mock do serviço
        serviceMock = new Mock<IDespesaService>();
        var service = serviceMock.Object; // objeto que está mockado

        // usar a configuração global do Mapster inicializada no MapsterMappingConfig
        var config = TypeAdapterConfig.GlobalSettings;
        mapper = new Mapper(config);

        // instanciar controller com dependências mockadas
        controller = new DespesaController(service);
    }
}
