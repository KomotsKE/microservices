using CoreLib.Interfaces;
using IdentityService.Dal.Entities;

namespace IdentityService.Dal.Interfaces;

/// <summary>
/// Интерфейс для работы с ролями пользователей.
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    /// <summary>Получить роль по имени.</summaryы>
    Task<Role?> GetRoleByName(string name);
}