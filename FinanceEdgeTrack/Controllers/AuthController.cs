using Asp.Versioning;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Auth;
using FinanceEdgeTrack.Application.Dtos.Write.Auth;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace FinanceEdgeTrack.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }


    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModelUserDTO loginModelDto)
    {
        var response = await _authService.Login(loginModelDto);

        if(!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelUserDTO registerModelDto)
    {
        var response = await _authService.Register(registerModelDto);

        if (!response.Success)
            return BadRequest(ResultMessages.InvalidCredentials);

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromQuery] TokenModelDTO tokenModelDto)
    {
        var response = await _authService.RefreshToken(tokenModelDto);

        if (!response.Success)
            return BadRequest(ResultMessages.InvalidAccessToken);

        return Ok(response);
    }


    [HttpPost("revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
        await _authService.Revoke(username);

        return Ok(ResultMessages.RevokeSuccessfull);
    }
}
