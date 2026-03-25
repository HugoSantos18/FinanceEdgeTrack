using FinanceEdgeTrack.Infrastructure.Config;
using Microsoft.EntityFrameworkCore;

namespace FinanceEdgeTrack.Infrastructure.Data;

public static class DataBaseConfiguration
{

    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services,IConfiguration config)
    {
        var dbSettings = new DataBaseSettings();
        var connection = config.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connection))
            throw new InvalidOperationException("DefaultConnection não configurada.");

        services.Configure<DataBaseSettings>(config.GetSection("ConnectionStrings"));

        services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connection));

        return services;
    }
}
