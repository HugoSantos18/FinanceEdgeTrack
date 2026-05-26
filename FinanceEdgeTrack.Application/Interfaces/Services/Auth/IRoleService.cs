using FinanceEdgeTrack.Application.DTOs.Read;

namespace FinanceEdgeTrack.Application.Interfaces.Services.Auth;

public interface IRoleService
{
    Task<ResponseDTO> CreateRoleAsync(string roleName);
    Task<ResponseDTO> AddUserToRoleAsync(string UserEmail, string roleName);
}
