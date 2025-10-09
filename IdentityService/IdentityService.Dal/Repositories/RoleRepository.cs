using IdentityService.Dal.Entities;
using IdentityService.Dal.Interfaces;
using IdentityService.Dal.DBContext;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Dal.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Role?> GetRoleByName(string name)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Name == name);
    }
}
