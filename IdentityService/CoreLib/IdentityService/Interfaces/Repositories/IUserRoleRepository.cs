using CoreLib.Entities;

namespace CoreLib.Interfaces;

/// <summary>
/// Интерфейс для управления связями пользователей и ролей.
/// </summary>
public interface IUserRoleRepository : IRepository<UserRole>
{
    /// <summary>Получить роли пользователя.</summary>
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId);

    /// <summary>Получить всех пользователей, имеющих указанную роль.</summary>
    Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId);

    /// <summary>Добавить роль пользователю.</summary>
    Task AddRoleToUserAsync(Guid userId, Guid roleId);

    /// <summary>Удалить роль у пользователя.</summary>
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);

    /// <summary>Проверить, есть ли у пользователя указанная роль.</summary>
    Task<bool> UserHasRoleAsync(Guid userId, string roleName);
}