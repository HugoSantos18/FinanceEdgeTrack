using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Application.Dtos.Read.Auth;
using FinanceEdgeTrack.Application.Dtos.Write.Auth;
using FinanceEdgeTrack.Application.Dtos.Write.Carteira;
using FinanceEdgeTrack.Domain.Interfaces;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using FinanceEdgeTrack.Domain.Interfaces.Services.CarteiraService;
using FinanceEdgeTrack.Domain.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinanceEdgeTrack.Application.Services.Auth;

public class AuthService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _uof;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private readonly ICarteiraService _carteiraService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ITokenService tokenService, UserManager<ApplicationUser> userManager,
                       IUnitOfWork uof, IMapper mapper, IConfiguration config, ICarteiraService carteiraService,
                       ILogger<AuthService> logger, ICurrentUser currentUser)
    {
        _uof = uof;
        _tokenService = tokenService;
        _userManager = userManager;
        _config = config;
        _mapper = mapper;
        _carteiraService = carteiraService;
        _logger = logger;
        _currentUser = currentUser;
    }


    public async Task<ApiResponse<LoginResponseDTO>> Login(LoginModelUserDTO loginModelDto)
    {
        var user = await _userManager.FindByNameAsync(loginModelDto.UserName!);

        if (user is null || !await _userManager.CheckPasswordAsync(user, loginModelDto.Password!))
        {
            _logger.LogInformation($"Usuário ou senha informados inválidos.");
            return ApiResponse<LoginResponseDTO>.Fail("Usuário ou senha inválidos.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
        {
            _logger.LogInformation($"Usuário já existe no sistema, não é possível cadastrar um novo com essas credenciais");
            return ApiResponse<ResponseDTO>.Fail(ResultMessages.UserAlreadyExists);
        }

        if (registerModelDto.Password != registerModelDto.ConfirmPassword)
        {
            _logger.LogInformation($"É necessário confirmar a senha de acordo com a senha informada.");
            return ApiResponse<ResponseDTO>.Fail(ResultMessages.ConfirmPasswordError);
        }

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
            
            _logger.LogInformation($"Erro ao criar um novo usuário: {errors}");
            return ApiResponse<ResponseDTO>.Fail($" Error: \n{errors}");
        }

        var carteiraDto = new CreateCarteiraDTO
        {
            UserId = user.Id,
            Saldo = 0m
        };

        var carteira = await _carteiraService.CreateAsync(carteiraDto);
        carteira.UserId = _currentUser.UserId;

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
            _logger.LogInformation($"Erro ao adicionar um novo refreshToken ao usuário, verifique as credenciais.");
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

        _logger.LogInformation($"Revoke {username}");
    }
}
