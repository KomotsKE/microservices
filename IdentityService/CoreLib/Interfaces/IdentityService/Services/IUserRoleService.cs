using CoreLib.DTOs;
namespace CoreLib.Interfaces;

public interface IUserRoleService
{
    Task AddRoleToUserAsync(Guid roleId, Guid userID);
    Task RemoveRoleFromUserAsync(Guid roleId, Guid userId);
    Task<IEnumerable<RoleDTO>> GetUserRolesAsync(Guid userId);
    Task<List<string>> GetUserRolesNamesAsync(Guid userId);
    Task UpdateUserRoleAsync (Guid userId, IEnumerable<Guid> roleId);
}