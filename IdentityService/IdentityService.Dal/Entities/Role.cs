using CoreLib.Entities;

namespace IdentityService.Dal.Entities;

/// <summary>
/// Представляет роль пользователя, определяющий его возможности
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// Строка с именем
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    /// Коллекция связей между пользователями и этой ролью.
    /// </summary>
    public List<UserRole> UserRoles { get; set; } = new();
}