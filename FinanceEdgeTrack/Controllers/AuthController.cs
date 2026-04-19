using Asp.Versioning;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read.Auth;
using FinanceEdgeTrack.Application.Dtos.Write.Auth;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Infrastructure.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger, 
                          UserManager<ApplicationUser> userManager, IRoleService roleService)
    {
        _authService = authService;
        _logger = logger;
        _roleService = roleService;
        _userManager = userManager;
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

    [HttpPost("make-admin")]
    [Authorize(Policy = Role.Admin)]
    public async Task<IActionResult> MakeAdmin([FromBody] MakeAdminDTO dto)
    {
        var currentAdmin = await _userManager.GetUserAsync(User);
        _logger.LogWarning("Admin {AdminEmail} está promovendo usuário {TargetEmail} para Admin",
            currentAdmin?.Email, dto.Email);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return NotFound("Usuário não encontrado");

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return BadRequest("Usuário já é Admin");

        var result = await _userManager.AddToRoleAsync(user, "Admin");

        if (result.Succeeded)
        {
            _logger.LogInformation("Usuário {UserEmail} promovido a Admin por {AdminEmail}",
                dto.Email, currentAdmin?.Email);

            return Ok(new { message = "Usuário promovido a Admin com sucesso" });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    [Route("AddUserToRole")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var result = await _roleService.AddUserToRole(email, roleName);

        if (result.Status == "400")
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    [Route("CreateRole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var result = await _roleService.CreateRole(roleName);

        if (result.Status == "400")
            return BadRequest(result);

        return Ok(result);
    }
}
