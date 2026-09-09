using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Auth;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.IsSuccess)
        {
            return Unauthorized(result.Error);
        }

        var loginData = result.Value;

        // 1. Set Refresh Token in HttpOnly cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = loginData.RefreshTokenExpiration
        };

        Response.Cookies.Append("refreshToken", loginData.RefreshToken, cookieOptions);

        // 2. Return Access Token & MedicalId in JSON response
        return Ok(new AuthResponse(
            loginData.AccessToken,
            loginData.RefreshToken
        ));
    }
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return HandleError(Error.Unauthorized("Auth.MissingRefreshToken", "Refresh token cookie is missing."));
        }
        var result = await _authService.RefreshTokenAsync(refreshToken);
        if (!result.IsSuccess)
        {
            return HandleError(result.Error);
        }
        SetRefreshTokenCookie(result.Value.RefreshToken);
        return Ok(new { accessToken = result.Value.AccessToken });
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.RevokeRefreshTokenAsync(refreshToken);
        }

        Response.Cookies.Delete("refreshToken");
        return Ok(new { message = "Logged out successfully" });
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Set to false if testing over unencrypted HTTP locally
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    private IActionResult HandleError(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => StatusCode(StatusCodes.Status404NotFound, error),
            ErrorType.Conflict => StatusCode(StatusCodes.Status409Conflict, error),
            ErrorType.Validation => StatusCode(StatusCodes.Status400BadRequest, error),
            ErrorType.Unauthorized => StatusCode(StatusCodes.Status401Unauthorized, error),
            ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, error),
            _ => StatusCode(StatusCodes.Status400BadRequest, error)
        };
    }
}