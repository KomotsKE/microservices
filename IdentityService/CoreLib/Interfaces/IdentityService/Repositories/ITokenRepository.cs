using CoreLib.Entities;
namespace CoreLib.Interfaces;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(Guid userId);
    Task RevokeAsync(RefreshToken refreshToken);
    Task DeleteExpiredAsync();
    Task<bool> IsActiveAsync(string token);
}