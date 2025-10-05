namespace CoreLib.DTOs;

public class CreateUserRequest
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "USER";
    public string PasswordHash { get; set; } = string.Empty;
}