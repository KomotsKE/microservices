using CoreLib.DTOs;
namespace CoreLib.Interfaces;

/// <summary>
    /// Интерфейс сервиса управления ролями пользователей.
    /// </summary>
    public interface IUserRoleService
    {
        /// <summary>Добавить роль пользователю.</summary>
        Task AddRoleToUserAsync(Guid roleId, Guid userID);

        /// <summary>Удалить роль у пользователя.</summary>
        Task RemoveRoleFromUserAsync(Guid roleId, Guid userId);

        /// <summary>Получить список ролей пользователя.</summary>
        Task<IEnumerable<RoleDTO>> GetUserRolesAsync(Guid userId);

        /// <summary>Получить список имен ролей пользователя.</summary>
        Task<List<string>> GetUserRolesNamesAsync(Guid userId);

        /// <summary>Обновить роли пользователя, заменив старые на указанные.</summary>
        Task UpdateUserRoleAsync(Guid userId, IEnumerable<Guid> roleIds);
    }