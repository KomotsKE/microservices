using CoreLib.Entities;

namespace CoreLib.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetRoleByName(string name);
}