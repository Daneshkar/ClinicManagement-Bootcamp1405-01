namespace ClinicManagement.Application.DTOs.Auth;

public record DoctorLoginRequest(string MedicalId, string Password)
{
    public string MedicalId { get; init; } = MedicalId?.Trim() ?? String.Empty;
}

public record AuthResponse(string AccessToken, string RefreshToken);

// Internal result object returned by IAuthService.LoginAsync
public record LoginServiceResult(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiration,
    string MedicalId
);