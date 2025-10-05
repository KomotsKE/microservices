using CoreLib.Entities;

namespace CoreLib.Interfaces;

public interface IUserRoleRepository : IRepository<UserRole>
{
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId);
    Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId);
    Task AddRoleToUserAsync(Guid userId, Guid roleId);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
    Task<bool> UserHasRoleAsync(Guid userId, string roleName);
}