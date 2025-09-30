using CoreLib.DTOs;

namespace CoreLib.Interfaces;

public interface IRoleService
{
    Task<RoleDTO> GetRoleAsync(Guid roleId);
    Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
    Task DeleteRole(Guid roleId);
    Task<RoleDTO> AddRole(CreateRoleRequest request);
    Task UpdateRole(RoleDTO dto);
}