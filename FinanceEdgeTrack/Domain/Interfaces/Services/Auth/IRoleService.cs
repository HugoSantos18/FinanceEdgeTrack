using FinanceEdgeTrack.Application.Dtos.Read;

namespace FinanceEdgeTrack.Domain.Interfaces.Services.Auth;

public interface IRoleService
{
    Task<ResponseDTO> CreateRole(string roleName);
    Task<ResponseDTO> AddUserToRole(string UserEmail, string roleName);
}
