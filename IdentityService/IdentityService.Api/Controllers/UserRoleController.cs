using CoreLib.DTOs;
using CoreLib.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("identityservice/api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class UserRoleController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;

    public UserRoleController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    /// <summary>
    /// Получить список ролей пользователя
    /// </summary>
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserRoles(Guid userId)
    {
        var roles = await _userRoleService.GetUserRolesAsync(userId);
        return Ok(roles);
    }

    /// <summary>
    /// Добавить роль пользователю
    /// </summary>
    [HttpPost("{userId:guid}/add/{roleId:guid}")]
    public async Task<IActionResult> AddRoleToUser(Guid userId, Guid roleId)
    {
        await _userRoleService.AddRoleToUserAsync(roleId, userId);
        return Ok(new { message = "Role added successfully." });
    }

    /// <summary>
    /// Удалить роль у пользователя
    /// </summary>
    [HttpDelete("{userId:guid}/remove/{roleId:guid}")]
    public async Task<IActionResult> RemoveRoleFromUser(Guid userId, Guid roleId)
    {
        await _userRoleService.RemoveRoleFromUserAsync(roleId, userId);
        return Ok(new { message = "Role removed successfully." });
    }

    /// <summary>
    /// Полностью обновить список ролей пользователя
    /// </summary>
    [HttpPut("{userId:guid}/update")]
    public async Task<IActionResult> UpdateUserRoles(Guid userId, [FromBody] IEnumerable<Guid> roleIds)
    {
        await _userRoleService.UpdateUserRoleAsync(userId, roleIds);
        return Ok(new { message = "User roles updated successfully." });
    }
}
