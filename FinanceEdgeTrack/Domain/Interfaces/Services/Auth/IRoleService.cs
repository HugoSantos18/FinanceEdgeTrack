using FinanceEdgeTrack.Application.Dtos.Read;

namespace FinanceEdgeTrack.Domain.Interfaces.Services.Auth;

public interface IRoleService
{
    Task<ResponseDTO> CreateRoleAsync(string roleName);
    Task<ResponseDTO> AddUserToRoleAsync(string UserEmail, string roleName);
}
