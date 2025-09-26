using CoreLib.Entities;

namespace CoreLib.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync (string email);
}