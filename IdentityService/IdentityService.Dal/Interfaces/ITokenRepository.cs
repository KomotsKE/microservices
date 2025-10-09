using IdentityService.Dal.Entities;
namespace IdentityService.Dal.Interfaces;

/// <summary>
/// Интерфейс для управления refresh-токенами.
/// </summary>
public interface ITokenRepository : IRepository<RefreshToken>
{
    /// <summary>Получить токен по его значению.</summary>
    Task<RefreshToken?> GetByTokenAsync(string token);

    /// <summary>Получить все токены пользователя.</summary>
    Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(Guid userId);

    /// <summary>Отозвать refresh-токен.</summary>
    Task RevokeAsync(RefreshToken refreshToken);

    /// <summary>Удалить все просроченные токены.</summary>
    Task DeleteExpiredAsync();

    /// <summary>Проверить, активен ли токен.</summary>
    Task<bool> IsActiveAsync(string token);
}