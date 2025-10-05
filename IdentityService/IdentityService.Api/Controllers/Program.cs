using System.Text;
using CoreLib.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var keyString = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration");;
var key = Encoding.UTF8.GetBytes(keyString);
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt configuration section is missing");

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    throw new InvalidOperationException("Jwt:Key is missing in configuration");

if (jwtSettings.AccessTokenExpirationMinutes <= 0)
    throw new InvalidOperationException("Jwt:AccessTokenExpirationMinutes must be greater than zero");

if (jwtSettings.RefreshTokenExpirationDays <= 0)
    throw new InvalidOperationException("Jwt:RefreshTokenExpirationDays must be greater than zero");

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // указывает, будет ли валидироваться издатель при валидации токена
        ValidateIssuer = true,
        // будет ли валидироваться потребитель токена
        ValidateAudience = true,
        // будет ли валидироваться время существования
        ValidateLifetime = true,
        // валидация ключа безопасности
        ValidateIssuerSigningKey = true,
        // строка, представляющая издателя
        ValidIssuer = jwtSettings["Issuer"],
        // установка потребителя токена
        ValidAudience = jwtSettings["Audience"],
        // установка ключа безопасности
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

app.UseAuthentication();

app.Run();
