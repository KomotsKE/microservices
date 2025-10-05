using CoreLib.Interfaces;
using CoreLib.Entities;
using CoreLib.DTOs;
using System.Globalization;
using CoreLib.Config;
namespace IdentityService.Logic;

public class TokenService : ITokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly JwtSettings _jwtSettings;
    private readonly UserRoleService _userRoleService;
    public TokenService(IRefreshTokenRepository refreshTokenRepository, IJwtProvider jwtProvider, JwtSettings jwtSettings, UserRoleService userRoleService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtProvider = jwtProvider;
        _jwtSettings = jwtSettings;
        _userRoleService = userRoleService;
    }

    public async Task<RefreshTokenDTO> CreateRefreshTokenAsync(Guid userId)
    {
        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays)
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

    public async Task<AuthResponse> GenerateAccessAndRefreshToken(Guid userId, string userEmail, List<string> roles)
    {
        var accessToken = _jwtProvider.GenerateToken(userId, userEmail, roles);
        var refreshTokenDTO = await CreateRefreshTokenAsync(userId);
        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenDTO.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes)
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

        var rolesNames = await _userRoleService.GetUserRolesNamesAsync(user.Id); 

        await RevokeRefreshTokenAsync(stored.Token);

        return await GenerateAccessAndRefreshToken(user.Id, user.Email, rolesNames);
    }
}
