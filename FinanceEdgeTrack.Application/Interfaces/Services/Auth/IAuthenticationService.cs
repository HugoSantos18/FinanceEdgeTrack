using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read;
using FinanceEdgeTrack.Application.DTOs.Read.Auth;
using FinanceEdgeTrack.Application.DTOs.Write.Auth;

namespace FinanceEdgeTrack.Application.Interfaces.Services.Auth;

public interface IAuthenticationService
{
    Task<ApiResponse<LoginResponseDTO>> Login(LoginModelUserDTO loginModelDto);
    Task<ApiResponse<ResponseDTO>> Register(RegisterModelUserDTO registerModelDto);
    Task<ApiResponse<TokenModelDTO>> RefreshToken(TokenModelDTO tokenDto);
    Task Revoke(string username);
}
