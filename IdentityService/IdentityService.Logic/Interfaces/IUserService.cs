using IdentityService.Logic.DTOs;
namespace IdentityService.Logic.Interfaces;

/// <summary>
/// Интерфейс сервиса управления пользователями.
/// </summary>
public interface IUserService
{
    /// <summary>Создать нового пользователя.</summary>
    Task<UserDto> CreateUser(CreateUserRequest request);

    /// <summary>Получить пользователя по идентификатору.</summary>
    Task<UserDto?> GetUserByIdAsync(Guid id);

    /// <summary>Получить всех пользователей.</summary>
    Task<IEnumerable<UserDto>> GetAllUsersAsync();

    /// <summary>Обновить данные пользователя.</summary>
    Task UpdateUserAsync(UserDto dto);

    /// <summary>Удалить пользователя по идентификатору.</summary>
    Task DeleteUserAsync(Guid id);

    /// <summary>Получить пользователя по email (с паролем).</summary>
    Task<UserWithPasswordDto?> GetUserByEmailAsync(string email);
}