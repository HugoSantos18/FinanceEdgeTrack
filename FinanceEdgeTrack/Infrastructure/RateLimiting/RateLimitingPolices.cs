
using FinanceEdgeTrack.Infrastructure.Config;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace FinanceEdgeTrack.Infrastructure.RateLimiting;

public static class RateLimitingPolices
{
    public static PartitionedRateLimiter<HttpContext> CreateUserLimiter(RateLimitingOptions options)
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(context =>
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier); // apenas para utilizar e ver se o user está authenticado.

            if (!string.IsNullOrEmpty(userId))
            {
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: $"user: {userId}",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = options.AuthenticatedUserLimit,
                        Window = TimeSpan.FromSeconds(options.WindowSeconds),
                        QueueLimit = 3,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
            }

            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknow";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: $"login: {ip}",
                factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = options.LoginLimit,
                Window = TimeSpan.FromSeconds(options.WindowSeconds),
                QueueLimit = 0
            });

        });
    }
}
