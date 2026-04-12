using FinanceEdgeTrack.Infrastructure.Config;

namespace FinanceEdgeTrack.Infrastructure.Extensions;

public static class RolesExtensionMiddleware
{
    public static IServiceCollection AddRolesPolicy(this IServiceCollection service)
    {
        string admRole = Role.Admin;
        string userRole = Role.User;

        service.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole(admRole));
            options.AddPolicy("User", policy => policy.RequireRole(userRole));
        });

        return service;
    }
}
