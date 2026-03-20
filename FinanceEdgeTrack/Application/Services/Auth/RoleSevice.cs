using FinanceEdgeTrack.Application.Common.Responses;
using FinanceEdgeTrack.Application.Dtos.Read;
using FinanceEdgeTrack.Domain.Interfaces.Services.Auth;
using FinanceEdgeTrack.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace FinanceEdgeTrack.Application.Services.Auth;

public class RoleSevice : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleSevice(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<ResponseDTO> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExist)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (roleResult.Succeeded)
            {
                return new ResponseDTO
                {
                    Status = $"200",
                    Message = $"Role {roleName} criada com sucesso!"
                };
            }
        }

        return new ResponseDTO
        {
            Status = $"400",
            Message = ResultMessages.InvalidIndentityRoleCreation
        };
    }

    public async Task<ResponseDTO> AddUserToRole(string UserEmail, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(UserEmail);

        if (user is not null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return new ResponseDTO
                {
                    Status = $"200",
                    Message = $"Role {roleName} adicionada a {user.UserName} com sucesso"
                };
            }
        }

        return new ResponseDTO
        {
            Status = "400",
            Message = ResultMessages.ErrorToAddUserToRole
        };
    }

}
