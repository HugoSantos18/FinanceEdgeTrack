namespace FinanceEdgeTrack.Application.Interfaces.Services.Auth;

public interface ICurrentUser
{
    string UserId { get; }
    string? Email { get;}
}
