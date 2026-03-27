namespace FinanceEdgeTrack.Domain.Interfaces.Services.Auth;

public class ICurrentUser
{
    Guid UserId { get; set; }
    string? Email { get; set; }
}
