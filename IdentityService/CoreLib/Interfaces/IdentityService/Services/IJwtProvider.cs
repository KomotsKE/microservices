namespace CoreLib.Interfaces;

/// <summary>
/// Интерфейс генератора JWT токенов.
/// </summary>
public interface IJwtProvider
{
    /// <summary>
    /// Генерация JWT-токена на основе данных пользователя и списка его ролей.
    /// </summary>
    string GenerateToken(Guid userId, string email, List<string> roles);
}