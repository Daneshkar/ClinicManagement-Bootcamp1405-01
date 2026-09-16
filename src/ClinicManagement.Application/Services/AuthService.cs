using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Auth;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;


namespace ClinicManagement.Application.Services;

public class AuthService : IAuthService

{
    private readonly IDoctorRepository _doctorRepository;
    private readonly ISecretaryRepository _secretaryRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public AuthService(
        IDoctorRepository doctorRepository,
        ISecretaryRepository secretaryRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _doctorRepository = doctorRepository;
        _secretaryRepository = secretaryRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }
    public async Task<Result<LoginServiceResult>> LoginAsync(LoginRequest request)
    {
        string userIdentifier;
        string name;
        string passwordHash;
        UserRole role;

        // 1. Try resolving as Doctor first
        var doctor = await _doctorRepository.GetByMedicalIdAsync(request.Identifier);
        if (doctor != null)
        {
            userIdentifier = doctor.MedicalId;
            name = doctor.Name;
            passwordHash = doctor.PasswordHash;
            role = UserRole.Doctor;
        }
        else
        {
            // 2. Fall back to Secretary lookup
            var secretary = await _secretaryRepository.GetByUsernameAsync(request.Identifier);
            if (secretary == null)
            {
                return Error.Unauthorized("Auth.InvalidCredentials", "Invalid credentials.");
            }
            userIdentifier = secretary.UserName;
            name = secretary.Name;
            passwordHash = secretary.PasswordHash;
            role = UserRole.Secretary;
        }
        // 3. Verify Password
        if (!_passwordHasher.IsMatch(request.Password, passwordHash))
        {
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid credentials.");
        }

        // 4. Issue Tokens
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(userIdentifier, name, role);
        var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();
        var newRefreshToken = RefreshToken.Create(userIdentifier, refreshTokenValue, TimeSpan.FromDays(7));

        await _refreshTokenRepository.AddAsync(newRefreshToken);
        return new LoginServiceResult(accessToken, refreshTokenValue, newRefreshToken.ExpiresAt, userIdentifier);
    }
    public async Task<Result<AuthResponse>> RefreshTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Error.Unauthorized("Auth.InvalidToken", "Invalid refresh token.");
        }

        var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
        if (refreshToken == null)
        {
            return Error.Unauthorized("Auth.InvalidToken", "Invalid refresh token.");
        }
        // Theft Detection: Attempting to use a revoked or already-used token

        if (refreshToken.IsRevoked || refreshToken.IsUsed)
        {
            await _refreshTokenRepository.RevokeAllForUserAsync(refreshToken.UserIdentifier);

            return Error.Unauthorized("Auth.TokenReuseDetected", "Security alert: Token reuse detected. Session invalidated.");
        }
        if (refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Error.Unauthorized("Auth.TokenExpired", "Refresh token has expired.");
        }
        // Resolve user role for new claims
        UserRole role;
        string name;
        var doctor = await _doctorRepository.GetByMedicalIdAsync(refreshToken.UserIdentifier);
        if (doctor != null)
        {
            role = UserRole.Doctor;
            name = doctor.Name;
        }
        else
        {
            var secretary = await _secretaryRepository.GetByUsernameAsync(refreshToken.UserIdentifier);
            if (secretary == null)
            {
                return Error.Unauthorized("Auth.UserNotFound", "User associated with token no longer exists.");
            }
            role = UserRole.Secretary;
            name = secretary.Name;
        }

        // Consume old token and emit new pair
        refreshToken.MarkAsUsed();
        await _refreshTokenRepository.UpdateAsync(refreshToken);

        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(refreshToken.UserIdentifier, name, role);
        var newRefreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();
        var newRefreshToken = RefreshToken.Create(refreshToken.UserIdentifier, newRefreshTokenValue, TimeSpan.FromDays(7));

        await _refreshTokenRepository.AddAsync(newRefreshToken);
        return new AuthResponse(newAccessToken, newRefreshTokenValue);

    }

    public async Task RevokeRefreshTokenAsync(string token)

    {
        if (string.IsNullOrWhiteSpace(token)) return;
        var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
        if (existingRefreshToken != null && !existingRefreshToken.IsRevoked)
        {
            existingRefreshToken.Revoke();
            await _refreshTokenRepository.UpdateAsync(existingRefreshToken);
        }
    }
}

