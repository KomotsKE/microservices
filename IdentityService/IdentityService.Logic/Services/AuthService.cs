using CoreLib.DTOs;
using CoreLib.Entities;
using CoreLib.Interfaces;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace IdentityService.Logic;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly UserRoleService _userRoleService;

    public AuthService(IUserService userService, ITokenService tokenService, IPasswordHasher passwordHasher, UserRoleService userRoleService)
    {
        _passwordHasher = passwordHasher;
        _userService = userService;
        _tokenService = tokenService;
        _userRoleService = userRoleService;
    }
    public async Task<UserDto> RegisterAsync(RegisterRequest request)
    {
        if (await _userService.GetUserByEmailAsync(request.Email) != null)
            throw new Exception("User already exists");

        var hashedPassword = _passwordHasher.Hash(request.Password);

        var createRequest = new CreateUserRequest
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = hashedPassword
        };

        var userDto = await _userService.CreateUser(createRequest);
        return userDto;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userService.GetUserByEmailAsync(request.Email);
        if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");
        var rolesNames = await _userRoleService.GetUserRolesNamesAsync(user.Id);
        var responce = await _tokenService.GenerateAccessAndRefreshToken(user.Id, user.Email, rolesNames);
        return responce;
    }   
}
