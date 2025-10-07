using Azure.Core;
using CoreLib.DTOs;
using CoreLib.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers;

[ApiController]
[Route("identityservice/api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /// <summary>
    /// Обновление токенов по refresh-токену
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshTokens([FromBody] TokenRequest  request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest("Refresh token is required.");

        var response = await _tokenService.RefreshTokensAsync(request.RefreshToken);
        return Ok(response);
    }

    /// <summary>
    /// Отзыв refresh-токена (выход из системы)
    /// </summary>
    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> RevokeToken([FromBody] TokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest("Refresh token is required.");

        await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);
        return NoContent();
    }
}
