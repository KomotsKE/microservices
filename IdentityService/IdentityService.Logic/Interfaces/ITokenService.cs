using IdentityService.Logic.DTOs;

namespace IdentityService.Logic.Interfaces;

/// <summary>
    /// Интерфейс сервиса для работы с токенами доступа и обновления.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>Создать новый refresh-токен для пользователя.</summary>
        Task<RefreshTokenDTO> CreateRefreshTokenAsync(Guid userId);

        /// <summary>Отозвать указанный refresh-токен.</summary>
        Task RevokeRefreshTokenAsync(string token);

        /// <summary>Обновить токены доступа по refresh-токену.</summary>
        Task<AuthResponse> RefreshTokensAsync(string refreshToken);

        /// <summary>
        /// Создать пару access + refresh токенов на основе данных пользователя.
        /// </summary>
        Task<AuthResponse> GenerateAccessAndRefreshToken(Guid userId, string userEmail, List<string> roles);
    }