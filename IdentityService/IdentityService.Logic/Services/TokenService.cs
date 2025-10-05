using CoreLib.Interfaces;
using CoreLib.Entities;
using CoreLib.DTOs;
using System.Globalization;
namespace IdentityService.Logic;
//TODO убрать лишние методы
public class TokenService : ITokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtProvider _jwtProvider;

    public TokenService(IRefreshTokenRepository refreshTokenRepository, IJwtProvider jwtProvider)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtProvider = jwtProvider;

    }

    public async Task<RefreshTokenDTO> CreateRefreshTokenAsync(Guid userId)
    {
        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = Guid.NewGuid().ToString(),
            //TODO сделать конфиг для jwt
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
        await _refreshTokenRepository.AddAsync(token);
        return new RefreshTokenDTO
        {
            Id = token.Id,
            UserId = token.UserId,
            Token = token.Token,
            ExpiresAt = token.ExpiresAt,
            IsRevoked = token.IsRevoked
        };
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token) ?? throw new Exception("Token not found");
        await _refreshTokenRepository.RevokeAsync(refreshToken);
    }

    public async Task<AuthResponse> GenerateAccessAndRefreshToken(Guid userId, string userEmail)
    {
        var accessToken = _jwtProvider.GenerateToken(userId, userEmail);
        var refreshTokenDTO = await CreateRefreshTokenAsync(userId);
        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenDTO.Token
        };
    }
    
    public async Task<AuthResponse> RefreshTokensAsync(string refreshToken)
    {
        var stored = await _refreshTokenRepository.GetByTokenAsync(refreshToken)
                    ?? throw new Exception("Invalid refresh token");

        if (stored.IsRevoked || stored.ExpiresAt <= DateTime.UtcNow)
            throw new Exception("Invalid refresh token");

        var user = stored.User ??
               throw new Exception("User not found for this refresh token");

        var newAccessToken = _jwtProvider.GenerateToken(user.Id, user.Email);

        var newRefreshTokenDTO = await CreateRefreshTokenAsync(stored.UserId);

        await RevokeRefreshTokenAsync(stored.Token);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenDTO.Token,
            //TODO добавить через сколько истекает через конфиг 
        };
    }
}
