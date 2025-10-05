using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoreLib.Config;
using CoreLib.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class JwtProvider : IJwtProvider
{
    private readonly JwtSettings _jwtSettings;
    public readonly int AccessTokenExpirationMinutes;
    public JwtProvider(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }
    
    public string GenerateToken(Guid userId, string email, List<string> roles)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeMinutes),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}