namespace FinanceEdgeTrack.Infrastructure.Config;

public class JwtSettings // recebe dados da Section "JWT"
{
    public string SecretKey { get; set; } = string.Empty;
    public string ValidAudience { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public int TokenValidityInMinutes { get; set; }
    public int RefreshTokenValidityInMinutes { get; set; }
}
