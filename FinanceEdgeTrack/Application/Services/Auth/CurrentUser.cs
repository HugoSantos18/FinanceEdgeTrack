using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FinanceEdgeTrack.Application.Services.Auth;

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
