using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using System.Security.Claims;

namespace FinanceEdgeTrack.Application.Services.Auth;

public class CurrentUser : ICurrentUser
{
    public Guid UserId { get;}
    public string Email { get; }

    public CurrentUser(IHttpContextAccessor accessor)
    {
        var user = accessor.HttpContext!.User;

        UserId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Email = user.FindFirstValue(ClaimTypes.Email)!;
    }
}
