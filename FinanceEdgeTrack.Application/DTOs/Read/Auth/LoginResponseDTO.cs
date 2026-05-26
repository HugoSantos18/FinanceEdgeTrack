namespace FinanceEdgeTrack.Application.DTOs.Read.Auth;

public class LoginResponseDTO
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime Expiration { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}
