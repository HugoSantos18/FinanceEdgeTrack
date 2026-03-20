using FinanceEdgeTrack.Infrastructure.Config;

namespace FinanceEdgeTrack.Infrastructure.Extensions;

public static class RolesExtensionMiddleware
{
    public static IServiceCollection AddRolesPolicy(this IServiceCollection service)
    {
        var admRole = Role.Admin;
        var userRole = Role.User;

        service.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole(admRole));
            options.AddPolicy("UserOnly", policy => policy.RequireRole(userRole));
        });

        return service;
    }
}
