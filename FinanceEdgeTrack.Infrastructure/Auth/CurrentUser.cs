using FinanceEdgeTrack.Application.Interfaces.Services.Auth;
using System.Security.Claims;

namespace FinanceEdgeTrack.Infrastructure.Auth;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;
    public string UserId => GetClaimGuid(ClaimTypes.NameIdentifier);
    public string Email => _accessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

    private string GetClaimGuid(string type)
    {
        var val = _accessor.HttpContext?.User?.FindFirstValue(type);
        return string.IsNullOrEmpty(val) ? String.Empty : val;
    }
}
