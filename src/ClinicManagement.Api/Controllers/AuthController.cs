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
    public async Task<IActionResult> Login(
        [FromBody] DoctorLoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            return Unauthorized(result.Error);
        }

        var loginData = result.Value;

        Response.Cookies.Append(
            "refreshToken",
            loginData.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = loginData.RefreshTokenExpiration
            });

        var response = new AuthResponse(
            loginData.AccessToken,
            loginData.RefreshToken);

        return Ok(response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return HandleError(
                Error.Failure(
                    "RefreshToken.Missing",
                    "Refresh token is missing."));
        }

        var result = await _authService.RefreshTokenAsync(refreshToken);

        if (!result.IsSuccess)
        {
            return HandleError(result.Error);
        }

        SetRefreshTokenCookie(result.Value.RefreshToken);

        return Ok(new
        {
            accessToken = result.Value.AccessToken
        });
    }

    [HttpPost("logout")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            await _authService.RevokeRefreshTokenAsync(refreshToken);
        }

        Response.Cookies.Delete("refreshToken");

        return Ok(new
        {
            message = "Logged out successfully"
        });
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append(
            "refreshToken",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });
    }

    private IActionResult HandleError(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => NotFound(error),
            ErrorType.Conflict => Conflict(error),
            ErrorType.Validation => BadRequest(error),
            _ => BadRequest(error)
        };
    }
}