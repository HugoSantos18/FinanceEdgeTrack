using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using System.Security.Claims;

namespace FinanceEdgeTrack.Application.Services.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId =>
        _httpContextAccessor.HttpContext?
        .User
        .FindFirst(ClaimTypes.NameIdentifier)?
        .Value!;

}
