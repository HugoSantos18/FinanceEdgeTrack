using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Application.Dtos.Read.Auth;
using FinanceEdgeTrack.Application.Dtos.Write.Auth;
using FinanceEdgeTrack.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinanceEdgeTrack.Domain.Interfaces.Services.Auth;

public interface IAuthenticationService
{
    Task<ApiResponse<LoginResponseDTO>> Login(LoginModelUserDTO loginModelDto);
    Task<ApiResponse<ResponseDTO>> Register(RegisterModelUserDTO registerModelDto);
    Task<ApiResponse<TokenModelDTO>> RefreshToken(TokenModelDTO tokenDto);
    Task Revoke(string username);
}
