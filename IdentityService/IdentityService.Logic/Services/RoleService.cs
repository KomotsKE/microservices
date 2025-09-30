using System.Net.Http.Headers;
using CoreLib.Entities;
using CoreLib.Interfaces;
using IdentityService.DAL.Repositories;

namespace IdentityService.Logic;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<RoleDTO> AddRole(CreateRoleRequest request)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
        };
        await _roleRepository.AddAsync(role);
        return new RoleDTO
        {
            Id = role.Id,
            Name = role.Name,
        };
    }

    public async Task DeleteRole(Guid roleId)
    {
        await _roleRepository.DeleteAsync(roleId);
    }

    public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Select(role => new RoleDTO
        {
            Id = role.Id,
            Name = role.Name,
        });
    }

    public async Task<RoleDTO> GetRoleAsync(Guid roleId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId) ?? throw new Exception("Role not found");
        return new RoleDTO
        {
            Id = role.Id,
            Name = role.Name,
        };
    }

    public async Task UpdateRole(RoleDTO dto)
    {
        var role = await _roleRepository.GetByIdAsync(dto.Id) ?? throw new Exception("Role not found");
        role.Name = dto.Name;
        await _roleRepository.UpdateAsync(role);
    }
}
