using FinanceEdgeTrack.Application.Dtos.Read.Auth;
using FinanceEdgeTrack.Application.Dtos.Write.Auth;
using FinanceEdgeTrack.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinanceEdgeTrack.Domain.Interfaces.Services;

public interface IAuthenticationService
{
    Task<ObjectResult> Login(LoginModelUserDTO loginModelDto);
    Task<ApplicationUserDTO> Register(RegisterModelUserDTO registerModelDto);
    Task<ObjectResult> RefreshToken(TokenModelDTO tokenDto);
    Task Revoke(string username);
}
