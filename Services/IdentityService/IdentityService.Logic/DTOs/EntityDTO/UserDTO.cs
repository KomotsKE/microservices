using IdentityService.Dal.Entities;

namespace IdentityService.Logic.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<Role> Roles { get; set; } = [];

}