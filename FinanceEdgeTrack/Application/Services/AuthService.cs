using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Application.Dtos.Read.Auth;
using FinanceEdgeTrack.Application.Dtos.Write.Auth;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Error;
using MapsterMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace FinanceEdgeTrack.Application.Services;

public class AuthService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _uof;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;


    public AuthService(ITokenService tokenService, UserManager<ApplicationUser> userManager,
                       IUnitOfWork uof, IMapper mapper, IConfiguration config)
    {
        _uof = uof;
        _tokenService = tokenService;
        _userManager = userManager;
        _config = config;
        _mapper = mapper;
    }

    public async Task<ObjectResult> Login(LoginModelUserDTO loginModelDto)
    {
        var user = await _userManager.FindByNameAsync(loginModelDto.UserName!);

        if (user is null || await _userManager.CheckPasswordAsync(user, loginModelDto.Password!))
            throw new InvalidOperationException(ResultMessages.InvalidLoginCredentials);

        var roles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var userRole in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = _tokenService.GenerateAccessToken(authClaims, _config);
        var refreshToken = _tokenService.GenerateRefreshToken();

        _ = int.TryParse(_config.GetSection("JWT").GetValue<string>("RefreshTokenValidityInMinutes"),
            out int refreshTokenValidityInMinutes);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpire = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            Expiration = token.ValidTo
        });

    }

    public async Task<ApplicationUserDTO> Register(RegisterModelUserDTO registerModelDto)
    {
        var userExists = await _userManager.FindByNameAsync(registerModelDto.UserName!);

        if (userExists is not null)
            throw new InvalidOperationException(ResultMessages.UserAlreadyExists);

        if (registerModelDto.Password != registerModelDto.ConfirmPassword)
            throw new InvalidOperationException(ResultMessages.ConfirmPasswordError);

        ApplicationUser user = new()
        {
            UserName = registerModelDto.UserName,
            Email = registerModelDto.Email,
            PasswordHash = registerModelDto.Password,
            PhoneNumber = registerModelDto.Telefone,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, registerModelDto.Password!);

        if (!result.Succeeded)
            throw new InvalidOperationException(ResultMessages.ErrorCreation);

        return _mapper.Map<ApplicationUserDTO>(user);
    }

    public async Task<ObjectResult> RefreshToken(TokenModelDTO tokenDto)
    {
        if (tokenDto is null)
            throw new ArgumentNullException(nameof(tokenDto));

        string? accessToken = tokenDto.AccessToken;
        string? refreshToken = tokenDto.RefreshToken;

        var principal = _tokenService.GetPrincipal(accessToken!, _config) ?? throw new InvalidOperationException(ResultMessages.InvalidAccessToken);

        string username = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(username);

        if (user is null || user.RefreshTokenExpire <= DateTime.Now || user.RefreshToken != refreshToken)
        {
            throw new InvalidOperationException(ResultMessages.InvalidRefreshToken);
        }

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _config);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;

        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken
        });
    }

    public async Task Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username) ?? throw new ArgumentException(ResultMessages.NotFoundUser);

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);
    }
}
