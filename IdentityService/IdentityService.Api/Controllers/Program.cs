using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
var app = builder.Build();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var keyString = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration");;
var key = Encoding.UTF8.GetBytes(keyString);

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

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
