namespace IdentityService.Logic.Interfaces;

/// <summary>
/// Интерфейс для безопасного хэширования паролей.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Создать хэш пароля.</summary>
    string Hash(string password);

    /// <summary>Проверить корректность пароля.</summary>
    bool Verify(string password, string storedHash);
}