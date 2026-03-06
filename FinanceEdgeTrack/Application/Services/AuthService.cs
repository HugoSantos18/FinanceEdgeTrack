using FinanceEdgeTrack.Application.Common;
using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Application.Dtos.Read.Auth;
using FinanceEdgeTrack.Application.Dtos.Write.Auth;
using FinanceEdgeTrack.Application.Dtos.Write.Carteira;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrack.Error;
using FinanceEdgeTrack.Infrastructure.Repositories;
using MapsterMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FinanceEdgeTrack.Application.Services;

public class AuthService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _uof;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private readonly ICarteiraService _carteiraService;


    public AuthService(ITokenService tokenService, UserManager<ApplicationUser> userManager,
                       IUnitOfWork uof, IMapper mapper, IConfiguration config, ICarteiraService carteiraService)
    {
        _uof = uof;
        _tokenService = tokenService;
        _userManager = userManager;
        _config = config;
        _mapper = mapper;
        _carteiraService = carteiraService;
    }

    public async Task<ApiResponse<LoginResponseDTO>> Login(LoginModelUserDTO loginModelDto)
    {
        var user = await _userManager.FindByNameAsync(loginModelDto.UserName!);

        if (user is null || !await _userManager.CheckPasswordAsync(user, loginModelDto.Password!))
        {
            var failResponse = new LoginResponseDTO()
            {
                Success = false,
                Message = ResultMessages.InvalidCredentials
            };

            return ApiResponse<LoginResponseDTO>.Fail("Usuário ou senha inválidos.");
        }

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

        return ApiResponse<LoginResponseDTO>.Ok(new LoginResponseDTO()
        {
            Success = true,
            Message = $"Olá {user.UserName} seja bem vindo ao Finance Edge Track!",
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            Expiration = token.ValidTo
        });
    }

    public async Task<ApiResponse<ResponseDTO>> Register(RegisterModelUserDTO registerModelDto)
    {
        var userExists = await _userManager.FindByNameAsync(registerModelDto.UserName!);

        if (userExists is not null)
            return ApiResponse<ResponseDTO>.Fail(ResultMessages.UserAlreadyExists);

        if (registerModelDto.Password != registerModelDto.ConfirmPassword)
            return ApiResponse<ResponseDTO>.Fail(ResultMessages.ConfirmPasswordError);

        ApplicationUser user = new()
        {
            UserName = registerModelDto.UserName,
            Email = registerModelDto.Email,
            PhoneNumber = registerModelDto.Telefone,
            SecurityStamp = Guid.NewGuid().ToString(),
            CPF = registerModelDto.CPF,
            DataNascimento = registerModelDto.DataNascimento,
        };

        var result = await _userManager.CreateAsync(user, registerModelDto.Password!);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            return ApiResponse<ResponseDTO>.Fail($" Error: \n{errors}");
        }

        var carteiraDto = new CreateCarteiraDTO
        {
            UserId = user.Id,
            Saldo = 0m
        };

        var carteira = await _carteiraService.CreateAsync(carteiraDto);
        user.CarteiraId = carteira.CarteiraId; 

        await _userManager.UpdateAsync(user);


        return ApiResponse<ResponseDTO>.Ok(new ResponseDTO()
        {
            Status = "200",
            Message = $"Usuário criado com sucesso. \nSeja bem vindo {user.UserName} ao Finance Edge Track!"
        });
    }

    public async Task<ApiResponse<TokenModelDTO>> RefreshToken(TokenModelDTO tokenDto)
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
            return ApiResponse<TokenModelDTO>.Fail(ResultMessages.InvalidRefreshToken);
        }

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _config);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;

        await _userManager.UpdateAsync(user);

        return ApiResponse<TokenModelDTO>.Ok(
            new TokenModelDTO()
            {
                AccessToken = newAccessToken.ToString(),
                RefreshToken = newRefreshToken,
            });
    }

    public async Task Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username) ?? throw new ArgumentException(ResultMessages.NotFoundUser);

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);
    }
}
