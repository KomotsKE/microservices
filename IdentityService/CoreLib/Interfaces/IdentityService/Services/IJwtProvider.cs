namespace CoreLib.Interfaces;

public interface IJwtProvider
{
    public string GenerateToken(Guid userId, string email);
    
}