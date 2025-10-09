using CoreLib.Entities;

namespace CoreLib.Interfaces;

/// <summary>
/// Интерфейс для работы с пользователями.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>Получить пользователя по email.</summary>
    Task<User?> GetByEmailAsync(string email);
}