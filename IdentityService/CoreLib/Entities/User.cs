namespace AuthService.Core.Entities;
public enum UserRole { USER, ADMIN};

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Name { get; set; } = null!;
    public UserRole Role { get; set; } = UserRole.USER;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}