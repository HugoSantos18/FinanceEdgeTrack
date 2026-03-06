using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface ITokenService
{
    JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration config);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipal(string token, IConfiguration config);

}
