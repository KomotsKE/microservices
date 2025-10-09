namespace IdentityService.Dal.Entities;

/// <summary>
/// Представляет пользователя сервиса
/// </summary>
public class User
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Электронная почта
    /// </summary>
    public string Email { get; set; } = null!;
    /// <summary>
    /// Захешированная строка пароля
    /// </summary>
    public string PasswordHash { get; set; } = null!;
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    /// Дата и время создания аккаунта
    /// </summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Дата и время последнего обновления пользователя
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    /// <summary>
    /// Список refresh-токенов выданных этому пользователю
    /// </summary>
    public List<RefreshToken> RefreshTokens { get; set; } = new();
    /// <summary>
    /// Коллекция связей между ролями и этим пользователем
    /// </summary>
    public List<UserRole> UserRoles { get; set; } = new();
}