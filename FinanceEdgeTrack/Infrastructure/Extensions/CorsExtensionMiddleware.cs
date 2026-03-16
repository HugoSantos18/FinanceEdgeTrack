using FinanceEdgeTrack.Infrastructure.Config;

namespace FinanceEdgeTrack.Infrastructure.Extensions;

public static class CorsExtensionMiddleware
{

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection service, IConfiguration config)
    {
        var corsOptions = config.GetSection("Cors")
                                .Get<CorsOptions>();

        service.AddCors(options =>
        {
            options.AddPolicy("DefaultAllowedCors", policy =>
            {
                policy
                .WithOrigins(corsOptions!.AllowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        return service;
    }
}
