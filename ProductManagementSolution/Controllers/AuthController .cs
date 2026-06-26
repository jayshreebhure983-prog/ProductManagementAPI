using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Exceptions;

namespace ProductManagement.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IConfiguration _configuration;


    public AuthController(IRefreshTokenService refreshTokenService ,IConfiguration configuration)
    {
        _refreshTokenService = refreshTokenService;
        _configuration = configuration;

    }

    /// <summary>
    /// Authenticates a user and returns JWT access and refresh tokens.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var userName = _configuration["AdminCredentials:UserName"];
        var password = _configuration["AdminCredentials:Password"];

        if (request.UserName != userName ||
            request.Password != password)
        {
            throw new UnauthorizedException("Invalid username or password.");
        }

        var response = await _refreshTokenService.LoginAsync(request.UserName);

        return Ok(response);
    }


    /// <summary>
    /// Rotates the refresh token and returns a new token pair.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var response = await _refreshTokenService.RefreshTokenAsync(request.RefreshToken);

        return Ok(response);
    }
}
