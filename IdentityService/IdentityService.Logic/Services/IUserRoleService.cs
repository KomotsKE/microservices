using CoreLib.DTOs;
using CoreLib.Entities;
using CoreLib.Interfaces;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
namespace IdentityService.Logic;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public UserRoleService(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
    }
    public async Task AddRoleToUserAsync(Guid roleId, Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new Exception("user not found");
        var role = await _roleRepository.GetByIdAsync(roleId) ?? throw new Exception("role not found");

        var exists = await _userRoleRepository.UserHasRoleAsync(userId, role.Name);
        if (exists)
            throw new Exception("Role already exists");

        await _userRoleRepository.AddRoleToUserAsync(userId, roleId);
    }

    public async Task<IEnumerable<RoleDTO>> GetUserRolesAsync(Guid userId)
    {
        var roles = await _userRoleRepository.GetRolesByUserIdAsync(userId);
        return roles.Select(r => new RoleDTO
        {
            Id = r.Id,
            Name = r.Name,
        });
    }

    public async Task RemoveRoleFromUserAsync(Guid roleId, Guid userId)
    {
        await _userRoleRepository.RemoveRoleFromUserAsync(userId, roleId);
    }

    public async Task UpdateUserRoleAsync(Guid userId, IEnumerable<Guid> roleIds)
    {
        var existingRoles = await _userRoleRepository.GetRolesByUserIdAsync(userId);
        var existingRolesId = existingRoles.Select(r => r.Id).ToList();

        var toRemove = existingRolesId.Where(id => !roleIds.Contains(id)).ToList();
        foreach (var id in toRemove)
            await _userRoleRepository.RemoveRoleFromUserAsync(userId, id);

        var toAdd = roleIds.Where(id => !existingRolesId.Contains(id)).ToList();
        foreach (var id in toAdd)
            await _userRoleRepository.AddRoleToUserAsync(userId, id);
    }
} 