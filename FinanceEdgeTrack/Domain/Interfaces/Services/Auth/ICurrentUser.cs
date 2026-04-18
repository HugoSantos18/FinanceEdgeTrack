namespace FinanceEdgeTrack.Domain.Interfaces.Services.Auth;

public interface ICurrentUser
{
    string UserId { get; }
    string? Email { get;}
}
