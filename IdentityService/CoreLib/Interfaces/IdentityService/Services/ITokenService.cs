using CoreLib.DTOs;
namespace CoreLib.Interfaces;

public interface ITokenService
{
    Task<RefreshTokenDTO> CreateRefreshTokenAsync(Guid userId);
    Task RevokeRefreshTokenAsync(string token);
    Task<AuthResponse> RefreshTokensAsync(string refreshToken);
    Task <AuthResponse> GenerateAccessAndRefreshToken (Guid userId, string userEmail);
}
