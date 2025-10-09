using CoreLib.DTOs;

namespace CoreLib.Interfaces;

/// <summary>
/// Интерфейс сервиса управления ролями.
/// </summary>
public interface IRoleService
{
    /// <summary>Получить роль по идентификатору.</summary>
    Task<RoleDTO> GetRoleAsync(Guid roleId);

    /// <summary>Получить список всех ролей.</summary>
    Task<IEnumerable<RoleDTO>> GetAllRolesAsync();

    /// <summary>Удалить роль по идентификатору.</summary>
    Task DeleteRole(Guid roleId);

    /// <summary>Создать новую роль.</summary>
    Task<RoleDTO> AddRole(CreateRoleRequest request);

    /// <summary>Обновить данные роли.</summary>
    Task UpdateRole(RoleDTO dto);
}