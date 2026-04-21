using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Infrastructure.Data;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrackxUnitTests.UnitTests;

public class DespesaUnitTestController
{
    public IUnitOfWork repository;
    public IMapper mapper;
    public static DbContextOptions<AppDbContext> dbContextOptions { get; }

   // public static string connectionString = DataBaseConfiguration.AddDatabaseConfiguration();
   // utilizar mocks para fazer a configuração de ambiente de teste sem deixar dados sensíveis e alterar configurações.

    static DespesaUnitTestController()
    {
        //dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
         //   .UseNpgsql();
    }
}
