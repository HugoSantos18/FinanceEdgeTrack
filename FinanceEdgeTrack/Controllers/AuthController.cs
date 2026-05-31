using Asp.Versioning;
using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.DTOs.Read;
using FinanceEdgeTrack.Application.DTOs.Read.Auth;
using FinanceEdgeTrack.Application.DTOs.Write.Auth;
using FinanceEdgeTrack.Application.Interfaces.Services.Auth;
using FinanceEdgeTrack.Infrastructure.Identity;
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


    /// <summary>Autentica um usuário e retorna os tokens de acesso (access token e refresh token).</summary>
    /// <param name="loginModelDto">Credenciais do usuário (UserName/Email e Password).</param>
    [HttpPost("Login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<TokenModelDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] LoginModelUserDTO loginModelDto)
    {
        var response = await _authService.Login(loginModelDto);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    /// <summary>Registra um novo usuário na plataforma.</summary>
    /// <param name="registerModelDto">Dados de cadastro: UserName, Email, Password, ConfirmPassword, Telefone, CPF e DataNascimento.</param>
    [HttpPost("Register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Register([FromBody] RegisterModelUserDTO registerModelDto)
    {
        var response = await _authService.Register(registerModelDto);

        if (!response.Success)
            return BadRequest(ResultMessages.InvalidCredentials);

        return Ok(response);
    }

    /// <summary>Renova o access token utilizando um refresh token válido. Requer role <b>Admin</b>.</summary>
    /// <param name="tokenModelDto">Par de tokens (AccessToken e RefreshToken) para renovação.</param>
    [HttpPost("refresh")]
    [Authorize(Policy = Role.Admin)]
    [ProducesResponseType(typeof(ApiResponse<TokenModelDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> RefreshToken([FromQuery] TokenModelDTO tokenModelDto)
    {
        var response = await _authService.RefreshToken(tokenModelDto);

        if (!response.Success)
            return BadRequest(ResultMessages.InvalidAccessToken);

        return Ok(response);
    }


    /// <summary>Revoga o refresh token de um usuário, forçando novo login. Requer role <b>Admin</b>.</summary>
    /// <param name="username">Nome de usuário cujo refresh token será revogado.</param>
    [HttpPost("revoke/{username}")]
    [Authorize(Policy = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Revoke(string username)
    {
        await _authService.Revoke(username);

        return Ok(ResultMessages.RevokeSuccessfull);
    }

    /// <summary>Promove um usuário ao role Admin. Requer role <b>Admin</b>.</summary>
    /// <param name="dto">E-mail do usuário a ser promovido.</param>
    [HttpPost("make-admin")]
    [Authorize(Policy = Role.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> MakeAdmin([FromBody] MakeAdminDTO dto)
    {
        var currentAdmin = await _userManager.GetUserAsync(User);
        _logger.LogWarning($"Admin {currentAdmin?.Email} está promovendo usuário {dto.Email} para Admin",
            currentAdmin?.Email, dto.Email);

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return NotFound(ResultMessages.NotFoundUser);

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return BadRequest(ResultMessages.UserAlreadyAdmin);

        var result = await _userManager.AddToRoleAsync(user, "Admin");

        if (result.Succeeded)
        {
            _logger.LogInformation("Usuário {UserId} promovido a Admin por {AdminId}",
                user.Id, currentAdmin?.Id);

            return Ok(new { message = ResultMessages.AdminMakedSuccessfully });
        }

        return BadRequest(result.Errors);
    }

    /// <summary>Adiciona um usuário a um role existente. Requer role <b>Admin</b>.</summary>
    /// <param name="email">E-mail do usuário.</param>
    /// <param name="roleName">Nome do role ao qual o usuário será adicionado.</param>
    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    [Route("AddUserToRole")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var result = await _roleService.AddUserToRoleAsync(email, roleName);

        if (result.Status == "400")
            return BadRequest(result);

        return Ok(result);
    }


    /// <summary>Cria um novo role no sistema. Requer role <b>Admin</b>.</summary>
    /// <param name="roleName">Nome do novo role a ser criado.</param>
    [HttpPost]
    [Authorize(Roles = Role.Admin)]
    [Route("CreateRole")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var result = await _roleService.CreateRoleAsync(roleName);

        if (result.Status == "400")
            return BadRequest(result);

        return Ok(result);
    }
}
