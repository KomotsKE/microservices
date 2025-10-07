using System.Text;
using CoreLib.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyApp.DAL;
using CoreLib.Interfaces;
using IdentityService.Logic;
using IdentityService.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();


var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt configuration section is missing");

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    throw new InvalidOperationException("Jwt:Key is missing in configuration");

if (jwtSettings.AccessTokenLifetimeMinutes <= 0)
    throw new InvalidOperationException("Jwt:AccessTokenExpirationMinutes must be greater than zero");

if (jwtSettings.RefreshTokenLifetimeDays <= 0)
    throw new InvalidOperationException("Jwt:RefreshTokenExpirationDays must be greater than zero");

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

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
        ValidIssuer = jwtSettings.Issuer,
        // установка потребителя токена
        ValidAudience = jwtSettings.Audience,
        // установка ключа безопасности
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
