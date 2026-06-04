using FinanceEdgeTrack.Infrastructure.Config;
using FinanceEdgeTrack.Infrastructure.RateLimiting;

namespace FinanceEdgeTrack.Extensions;

public static class RateLimitingExtensionMiddleware
{
    public static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services, IConfiguration config)
    {
        var options = new RateLimitingOptions();

        config.GetSection(RateLimitingOptions.SectionName)
              .Bind(options);

        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.GlobalLimiter = RateLimitingPolices.CreateUserLimiter(options);

            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        return services;
    }
}
