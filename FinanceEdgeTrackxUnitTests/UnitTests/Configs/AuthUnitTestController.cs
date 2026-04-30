
using FinanceEdgeTrack.Controllers;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using FinanceEdgeTrack.Domain.Models;
using MapsterMapper;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.Extensions.Logging;

namespace FinanceEdgeTrackxUnitTests.UnitTests.Configs;

public class AuthUnitTestController
{
    public AuthController controller;
    public IMapper mapper;
    public Mock<IAuthenticationService> authServiceMock;
    public Mock<IRoleService> roleServiceMock;
    public Mock<UserManager<ApplicationUser>> userManagerServiceMock;
    public Mock<ILogger<AuthController>> loggerMock;

    public AuthUnitTestController()
    {
        var config = TypeAdapterConfig.GlobalSettings;
        mapper = new Mapper(config);

        authServiceMock = new Mock<IAuthenticationService>();
        var authService = authServiceMock.Object;

        roleServiceMock = new Mock<IRoleService>();
        var roleService = roleServiceMock.Object;

        var userStore = new Mock<IUserStore<ApplicationUser>>();
        userManagerServiceMock = new Mock<UserManager<ApplicationUser>>(
            userStore.Object,
            null, null, null, null, null, null, null, null);

        loggerMock = new Mock<ILogger<AuthController>>();
        var logger = loggerMock.Object; 

        controller = new AuthController(authService, logger, userManagerServiceMock.Object, roleService);
    }
}
