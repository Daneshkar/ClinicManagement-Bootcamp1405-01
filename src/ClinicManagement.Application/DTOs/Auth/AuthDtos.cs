namespace ClinicManagement.Application.DTOs.Auth;

public record LoginRequest(
    string Identifier,
    string Password
)
{
    public string Identifier { get; init; } = Identifier.Trim();
}

public record AuthResponse(
    string AccessToken,
    string RefreshToken
);

public record LoginServiceResult(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiration,
    string UserIdentifier
);

