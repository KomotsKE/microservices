using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoreLib.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class JwtProvider : IJwtProvider
{
    private readonly IConfiguration _config;
    public JwtProvider(IConfiguration config)
    {
        _config = config;
    }
    
    public string GenerateToken(Guid userId, string email)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var keyString = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration");
        var key = Encoding.UTF8.GetBytes(keyString);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email)
        };

        var accessTokenExpirationMinutesValue = int.Parse(jwtSettings["AccessTokenExpirationMinutes"]
        ?? throw new InvalidOperationException("AccessTokenExpirationMinutes is missing in configuration"));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(accessTokenExpirationMinutesValue),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}