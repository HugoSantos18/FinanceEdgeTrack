namespace FinanceEdgeTrack.Domain.Interfaces.Services.Auth;

public interface ICurrentUser
{
    Guid UserId { get; }
    string? Email { get;}
}
