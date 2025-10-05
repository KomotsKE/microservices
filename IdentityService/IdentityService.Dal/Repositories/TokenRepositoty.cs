using CoreLib.Entities;
using CoreLib.Interfaces;
using Microsoft.EntityFrameworkCore;
using MyApp.DAL;

namespace IdentityService.DAL.Repositories;

public class TokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public TokenRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task DeleteExpiredAsync()
    {
        var expiredTokens = _context.RefreshTokens.Where(r => r.ExpiresAt <= DateTime.UtcNow);
        _context.RefreshTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RefreshToken>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens
        .Where(r => r.UserId == userId).ToListAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
        .FirstOrDefaultAsync(r => r.Token == token);
    }

    public async Task<bool> IsActiveAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token);

        return refreshToken is not null &&
               !refreshToken.IsRevoked &&
               refreshToken.ExpiresAt > DateTime.UtcNow;
    }

    public async Task RevokeAsync(RefreshToken refreshToken)
    {
        refreshToken.IsRevoked = true;
        _context.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
}
