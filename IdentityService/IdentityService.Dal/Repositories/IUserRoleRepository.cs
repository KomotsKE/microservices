using CoreLib.Interfaces;
using CoreLib.Entities;
using MyApp.DAL;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DAL.Repositories;

public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
{
    private readonly ApplicationDbContext _context;
    public UserRoleRepository(ApplicationDbContext context) : base(context) 
    {
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId)
    {
        return await _context.UserRoles
        .Where(ur => ur.UserId == userId)
        .Include(ur => ur.Role)
        .Select(ur => ur.Role)
        .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId)
    {
        return await _context.UserRoles
        .Where(ur => ur.RoleId == roleId)
        .Include(ur => ur.User)
        .Select(ur => ur.User)
        .ToListAsync();
    }

    public async Task AddRoleToUserAsync(Guid userId, Guid roleId)
    {
        var exists = await _context.UserRoles
        .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (!exists)
        {
            await _context.UserRoles.AddAsync(new UserRole
            {
                UserId = userId,
                RoleId = roleId
            });
            await _context.SaveChangesAsync();
        }

    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId)
    {
        var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        if (userRole != null)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, string roleName)
    {
        return await _context.UserRoles
        .Include(ur => ur.Role)
        .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);
    }
}