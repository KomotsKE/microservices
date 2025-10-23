using IdentityService.Logic.DTOs;
using IdentityService.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("identityservice/api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Получить список всех ролей
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Получить роль по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRoleById(Guid id)
    {
        var role = await _roleService.GetRoleAsync(id);
        return Ok(role);
    }

    /// <summary>
    /// Создать новую роль
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Role name cannot be empty.");

        var role = await _roleService.AddRole(request);
        return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
    }

    /// <summary>
    /// Обновить роль
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleDTO dto)
    {
        if (id != dto.Id)
            return BadRequest("Role ID mismatch.");

        await _roleService.UpdateRole(dto);
        return NoContent();
    }

    /// <summary>
    /// Удалить роль
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        await _roleService.DeleteRole(id);
        return NoContent();
    }
}
