using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Application.Dtos.Read.Auth;
using FinanceEdgeTrack.Application.Dtos.Write.Auth;
using FinanceEdgeTrack.Domain.Models;
using FinanceEdgeTrackxUnitTests.UnitTests.Configs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Auth;

public class AuthUnitTest
{
    private readonly AuthUnitTestController _helper;

    public AuthUnitTest()
    {
        _helper = new AuthUnitTestController();
    }


    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsAreValid()
    {
        // arrange
        var loginModel = new LoginModelUserDTO { UserName = "User", Email = "usertest@gmail.com", Password = "StrongPassword123@" };
        var mapped = _helper.mapper.Map<LoginResponseDTO>(loginModel);
        _helper.authServiceMock
            .Setup(s => s.Login(loginModel))
            .ReturnsAsync(ApiResponse<LoginResponseDTO>.Ok(mapped));

        // act
        var result = await _helper.controller.Login(loginModel);

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<LoginResponseDTO>>(ok.Value);
        Assert.True(response.Success);
        _helper.authServiceMock.Verify(s => s.Login(loginModel), Times.Once);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenServiceFails()
    {
        // arrange
        var loginModel = new LoginModelUserDTO { UserName = "", Email = "usertest@gmail.com", Password = "notsecretpassword" };
        _helper.authServiceMock
               .Setup(s => s.Login(loginModel))
               .ReturnsAsync(ApiResponse<LoginResponseDTO>.Fail("Invalid credentials, unauthorized"));

        // act
        var result = await _helper.controller.Login(loginModel);

        // assert
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = Assert.IsType<ApiResponse<LoginResponseDTO>>(unauthorized.Value);
        Assert.False(response.Success);
        _helper.authServiceMock.Verify(s => s.Login(loginModel), Times.Once);
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenCredentialsAreValid()
    {
        var registerModel = new RegisterModelUserDTO { UserName = "Test user", Email = "usertest@gmail.com", Password = "StrongPassword123@", ConfirmPassword = "StrongPassword123@", Telefone = "31999990000", CPF = "000.000.000-00", DataNascimento = new DateTime(2003, 05, 12) };
        var mapped = _helper.mapper.Map<ResponseDTO>(registerModel);
        _helper.authServiceMock
               .Setup(s => s.Register(registerModel))
               .ReturnsAsync(ApiResponse<ResponseDTO>.Ok(mapped));

        var result = await _helper.controller.Register(registerModel);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<ResponseDTO>>(ok.Value);
        Assert.True(response.Success);
        Assert.Equal(mapped.Status, response.Data!.Status);
        _helper.authServiceMock.Verify(s => s.Register(registerModel), Times.Once);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenCredentialsAreInvalidAndServiceFails()
    {
        var registerModel = new RegisterModelUserDTO { UserName = "", Email = "usertest@gmail.com", Password = "notstrongpassword", ConfirmPassword = "notstrongpasswor", Telefone = "31999990000", CPF = "000.000.000-00", DataNascimento = new DateTime(2003, 05, 12) };
        _helper.authServiceMock
               .Setup(s => s.Register(registerModel))
               .ReturnsAsync(ApiResponse<ResponseDTO>.Fail("ResultMessage"));

        var result = await _helper.controller.Register(registerModel);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<string>(bad.Value);
        Assert.Equal(ResultMessages.InvalidCredentials, response);
        _helper.authServiceMock.Verify(s => s.Register(registerModel), Times.Once);
    }


    [Fact]
    public async Task RefreshToken_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var tokenModel = new TokenModelDTO { AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0.KMUFsIDTnFmyG3nMiGM6H9FNFUROf3wh7SmqJp-QV30", RefreshToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0X3VzZXJfMTIzNDUiLCJpYXQiOjE3MzQ1Njc4OTAsImV4cCI6MTczNzE1OTg5MCwianRpIjoiNzI5M2I4Y2UtYzE0ZS00YzI5LWE5ZWItYzUwYjU2ZTk4ZTQ2In0.dGhpcy1pcy1hLXJhbmRvbS1zaWduYXR1cmUtZm9yLXRlc3Rpbmc" };
        _helper.authServiceMock
               .Setup(s => s.RefreshToken(tokenModel))
               .ReturnsAsync(ApiResponse<TokenModelDTO>.Ok(tokenModel));

        var result = await _helper.controller.RefreshToken(tokenModel);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ApiResponse<TokenModelDTO>>(ok.Value);
        Assert.True(response.Success);
        _helper.authServiceMock.Verify(s => s.RefreshToken(tokenModel), Times.Once);
    }

    [Fact]
    public async Task RefreshToken_ReturnsBadRequest_WhenServiceFails()
    {
        var tokenModel = new TokenModelDTO { AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0.KMUFsIDTnFmyG3nMiGM6H9FNFUROf3wh7SmqJp-l", RefreshToken = "" };
        _helper.authServiceMock
               .Setup(s => s.RefreshToken(tokenModel))
               .ReturnsAsync(ApiResponse<TokenModelDTO>.Fail(ResultMessages.InvalidRefreshToken));

        var result = await _helper.controller.RefreshToken(tokenModel);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<string>(bad.Value);
        Assert.Equal(ResultMessages.InvalidAccessToken, response);
        _helper.authServiceMock.Verify(s => s.RefreshToken(tokenModel), Times.Once);
    }


    [Fact]
    public async Task RevokeToken_ReturnsOk_WhenServiceReturnsSuccess()
    {
        string username = "UserTest";
        _helper.authServiceMock
               .Setup(s => s.Revoke(username));

        var result = await _helper.controller.Revoke(username);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<string>(ok.Value);
        Assert.Equal(ResultMessages.RevokeSuccessfull, response);
        _helper.authServiceMock.Verify(s => s.Revoke(username), Times.Once);
    }

    [Fact]
    public async Task MakeAdmin_ReturnsOk_WhenServiceReturnsSuccess()
    {
        var makeAdmDto = new MakeAdminDTO { Email = "admemail@gmail.com" };

        var currentAdmin = new ApplicationUser { Email = "current@admin.com", UserName = "currentAdmin" };
        var targetUser = new ApplicationUser { Email = makeAdmDto.Email, UserName = "targetUser" };

        _helper.userManagerServiceMock
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(currentAdmin);

        _helper.userManagerServiceMock
            .Setup(um => um.FindByEmailAsync(makeAdmDto.Email))
            .ReturnsAsync(targetUser);

        _helper.userManagerServiceMock
            .Setup(um => um.IsInRoleAsync(targetUser, "Admin"))
            .ReturnsAsync(false);

        _helper.userManagerServiceMock
            .Setup(um => um.AddToRoleAsync(targetUser, "Admin"))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _helper.controller.MakeAdmin(makeAdmDto);

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = ok.Value as dynamic;
        Assert.Equal(ResultMessages.AdminMakedSuccessfully, (string)value.message);
        _helper.userManagerServiceMock.Verify(um => um.AddToRoleAsync(targetUser, "Admin"), Times.Once);
    }

    [Fact]
    public async Task MakeAdmin_ReturnsNotFound_WhenUserNotFound()
    {
        var makeAdmDto = new MakeAdminDTO { Email = "" };
        _helper.userManagerServiceMock
               .Setup(s => s.FindByEmailAsync(makeAdmDto.Email))
               .ReturnsAsync(It.IsAny<ApplicationUser>());

        var result = await _helper.controller.MakeAdmin(makeAdmDto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<string>(notFound.Value);
        Assert.Equal(ResultMessages.NotFoundUser, response);
    }

    [Fact]
    public async Task MakeAdmin_ReturnsBadRequest_WhenServiceFails()
    {
        var makeAdmDto = new MakeAdminDTO { Email = "admemail@gmail.com" };

        var currentAdmin = new ApplicationUser { Email = "current@admin.com", UserName = "currentAdmin" };
        var targetUser = new ApplicationUser { Email = makeAdmDto.Email, UserName = "targetUser" };

        _helper.userManagerServiceMock
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(currentAdmin);

        _helper.userManagerServiceMock
            .Setup(um => um.FindByEmailAsync(makeAdmDto.Email))
            .ReturnsAsync(targetUser);

        _helper.userManagerServiceMock
            .Setup(um => um.IsInRoleAsync(targetUser, "Admin"))
            .ReturnsAsync(true);

        var result = await _helper.controller.MakeAdmin(makeAdmDto);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<string>(bad.Value);
        Assert.Equal(ResultMessages.UserAlreadyAdmin, response);
        _helper.userManagerServiceMock.Verify(um => um.IsInRoleAsync(targetUser, "Admin"), Times.Once);
    }

    [Fact]
    public async Task AddUserToRole_ReturnsOk_WhenUserAddToRoleSuccess()
    {
        string email = "admsecundaryemail@gmail.com";
        string roleName = "Admin";
        _helper.roleServiceMock
               .Setup(s => s.AddUserToRoleAsync(email, roleName))
               .ReturnsAsync(new ResponseDTO { Status = "200", Message = $"Role {roleName} adicionada ao user" });

        var result = await _helper.controller.AddUserToRole(email, roleName);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ResponseDTO>(ok.Value);
        Assert.True(response.Status == "200");
        _helper.roleServiceMock.Verify(s => s.AddUserToRoleAsync(email, roleName), Times.Once);
    }

    [Fact]
    public async Task AddUserToRole_ReturnsBadRequest_WhenServiceFails()
    {
        string email = "";
        string roleName = "NewRole";
        _helper.roleServiceMock
               .Setup(s => s.AddUserToRoleAsync(email, roleName))
               .ReturnsAsync(new ResponseDTO { Status = "400", Message = ResultMessages.ErrorToAddUserToRole });

        var result = await _helper.controller.AddUserToRole(email, roleName);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ResponseDTO>(bad.Value);
        Assert.True(response.Status == "400");
        Assert.Equal(ResultMessages.ErrorToAddUserToRole, response.Message);
        _helper.roleServiceMock.Verify(s => s.AddUserToRoleAsync(email, roleName), Times.Once);
    }

    [Fact]
    public async Task CreateRole_ReturnsOk_WhenRoleCreatedSuccessfully()
    {
        string roleName = "NewRole";
        _helper.roleServiceMock
               .Setup(s => s.CreateRoleAsync(roleName))
               .ReturnsAsync(new ResponseDTO { Status = "200", Message = $"Role {roleName} criada com sucesso!" });

        var result = await _helper.controller.CreateRole(roleName);

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ResponseDTO>(ok.Value);
        Assert.True(response.Status == "200");
        _helper.roleServiceMock.Verify(s => s.CreateRoleAsync(roleName), Times.Once);
    }

    [Fact]
    public async Task CreateRole_ReturnsBadRequest_WhenServiceFails()
    {
        string roleName = "NewRole";
        _helper.roleServiceMock
               .Setup(s => s.CreateRoleAsync(It.IsAny<string>()))
               .ReturnsAsync(new ResponseDTO { Status = "400", Message = ResultMessages.InvalidIndentityRoleCreation });

        var result = await _helper.controller.CreateRole(roleName);

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ResponseDTO>(bad.Value);
        Assert.True(response.Status == "400");
        Assert.Equal(ResultMessages.InvalidIndentityRoleCreation, response.Message);
        _helper.roleServiceMock.Verify(s => s.CreateRoleAsync(roleName), Times.Once);
    }

}
