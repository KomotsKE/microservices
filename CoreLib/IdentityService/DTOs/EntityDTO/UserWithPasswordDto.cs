using CoreLib.Entities;

namespace CoreLib.DTOs;

public class UserWithPasswordDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<Role> Roles { get; set; } = [];
    public string PasswordHash { get; set; } = string.Empty;
}