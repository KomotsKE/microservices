using IdentityService.Logic.DTOs;

namespace IdentityService.Logic.Interfaces;

/// <summary>
/// Интерфейс сервиса аутентификации.
/// </summary>
public interface IAuthService
{
    /// <summary>Регистрация нового пользователя.</summary>
    Task<UserDto> RegisterAsync(RegisterRequest request);

    /// <summary>Аутентификация пользователя и выдача токенов.</summary>
    Task<AuthResponse> LoginAsync(LoginRequest request);
}