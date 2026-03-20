using Asp.Versioning;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Auth;
using FinanceEdgeTrack.Application.Dtos.Write.Auth;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using FinanceEdgeTrack.Infrastructure.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceEdgeTrack.Controllers;


[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IRoleService _roleService;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger, IRoleService roleService)
    {
        _authService = authService;
        _logger = logger;
        _roleService = roleService;
    }


    [HttpPost("Login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModelUserDTO loginModelDto)
    {
        var response = await _authService.Login(loginModelDto);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterModelUserDTO registerModelDto)
    {
        var response = await _authService.Register(registerModelDto);

        if (!response.Success)
            return BadRequest(ResultMessages.InvalidCredentials);

        return Ok(response);
    }

    [HttpPost("refresh")]
    [Authorize(Policy = Role.Admin)]
    public async Task<IActionResult> RefreshToken([FromQuery] TokenModelDTO tokenModelDto)
    {
        var response = await _authService.RefreshToken(tokenModelDto);

        if (!response.Success)
            return BadRequest(ResultMessages.InvalidAccessToken);

        return Ok(response);
    }


    [HttpPost("revoke/{username}")]
    [Authorize(Policy = Role.Admin)]
    public async Task<IActionResult> Revoke(string username)
    {
        await _authService.Revoke(username);

        return Ok(ResultMessages.RevokeSuccessfull);
    }


    [HttpPost]
    [Authorize(Policy = Role.Admin)]
    [Route("AddUserToRole")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var result = await _roleService.AddUserToRole(email, roleName);

        if (result.Status == "400")
        {
            return BadRequest(result);
        }

        return Ok(result);
    }


    [HttpPost]
    [Authorize(Policy = Role.Admin)]
    [Route("CreateRole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var result = await _roleService.CreateRole(roleName);

        if (result.Status == "400")
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
