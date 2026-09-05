using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Auth;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;

namespace ClinicManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly ISecretaryRepository _secretaryRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthService(
        IDoctorRepository doctorRepository,
        ISecretaryRepository secretaryRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IValidator<LoginRequest> loginValidator)
    {
        _doctorRepository = doctorRepository;
        _secretaryRepository = secretaryRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _loginValidator = loginValidator;
    }

    public async Task<Result<LoginServiceResult>> LoginAsync(LoginRequest request)
    {
        var validationResult = await _loginValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return FormatValidationErrors(validationResult.Errors);
        }

        var doctor = await _doctorRepository.GetByMedicalIdAsync(request.Identifier);

        if (doctor is not null)
        {
            if (!_passwordHasher.IsMatch(request.Password, doctor.PasswordHash))
            {
                return Error.Unauthorized(
                    "Auth.InvalidCredentials",
                    "Invalid medical ID or password.");
            }

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(
                doctor.MedicalId,
                doctor.Name,
                UserRole.Doctor);

            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var newRefreshToken = RefreshToken.Create(
                doctor.MedicalId,
                refreshToken,
                TimeSpan.FromDays(7));

            await _refreshTokenRepository.AddAsync(newRefreshToken);

            return new LoginServiceResult(
                accessToken,
                refreshToken,
                newRefreshToken.ExpiresAt,
                doctor.MedicalId);
        }

        var secretary = await _secretaryRepository
            .GetByUsernameAsync(request.Identifier);

        if (secretary is null)
        {
            return Error.Unauthorized(
                "Auth.InvalidCredentials",
                "Invalid username or password.");
        }

        if (!_passwordHasher.IsMatch(
                request.Password,
                secretary.PasswordHash))
        {
            return Error.Unauthorized(
                "Auth.InvalidCredentials",
                "Invalid username or password.");
        }

        var secretaryAccessToken = _jwtTokenGenerator.GenerateAccessToken(
            secretary.UserName,
            secretary.Name,
            UserRole.Secretary);

        var secretaryRefreshToken =
            _jwtTokenGenerator.GenerateRefreshToken();

        var newSecretaryRefreshToken = RefreshToken.Create(
            secretary.UserName,
            secretaryRefreshToken,
            TimeSpan.FromDays(7));

        await _refreshTokenRepository.AddAsync(
            newSecretaryRefreshToken);

        return new LoginServiceResult(
            secretaryAccessToken,
            secretaryRefreshToken,
            newSecretaryRefreshToken.ExpiresAt,
            secretary.UserName);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Error.Unauthorized(
                "Auth.InvalidToken",
                "Invalid or expired refresh token.");
        }

        var refreshToken =
            await _refreshTokenRepository.GetByTokenAsync(token);

        if (refreshToken == null ||
            refreshToken.IsRevoked ||
            refreshToken.IsUsed ||
            refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Error.Unauthorized(
                "Auth.InvalidToken",
                "Invalid or expired refresh token.");
        }

        var doctor = await _doctorRepository
            .GetByMedicalIdAsync(refreshToken.UserIdentifier);

        if (doctor is not null)
        {
            refreshToken.MarkAsUsed();
            await _refreshTokenRepository.UpdateAsync(refreshToken);

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(
                doctor.MedicalId,
                doctor.Name,
                UserRole.Doctor);

            var newRefreshTokenValue =
                _jwtTokenGenerator.GenerateRefreshToken();

            var newRefreshToken = RefreshToken.Create(
                doctor.MedicalId,
                newRefreshTokenValue,
                TimeSpan.FromDays(7));

            await _refreshTokenRepository.AddAsync(newRefreshToken);

            return new AuthResponse(
                accessToken,
                newRefreshTokenValue);
        }

        var secretary = await _secretaryRepository
            .GetByUsernameAsync(refreshToken.UserIdentifier);

        if (secretary is null)
        {
            return Error.Unauthorized(
                "Auth.InvalidCredentials",
                "Invalid user.");
        }

        refreshToken.MarkAsUsed();
        await _refreshTokenRepository.UpdateAsync(refreshToken);

        var secretaryAccessToken = _jwtTokenGenerator.GenerateAccessToken(
            secretary.UserName,
            secretary.Name,
            UserRole.Secretary);

        var newSecretaryRefreshTokenValue =
            _jwtTokenGenerator.GenerateRefreshToken();

        var newSecretaryRefreshToken = RefreshToken.Create(
            secretary.UserName,
            newSecretaryRefreshTokenValue,
            TimeSpan.FromDays(7));

        await _refreshTokenRepository.AddAsync(
            newSecretaryRefreshToken);

        return new AuthResponse(
            secretaryAccessToken,
            newSecretaryRefreshTokenValue);
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        var existingRefreshToken =
            await _refreshTokenRepository.GetByTokenAsync(token);

        if (existingRefreshToken != null &&
            !existingRefreshToken.IsRevoked)
        {
            existingRefreshToken.Revoke();
            await _refreshTokenRepository.UpdateAsync(
                existingRefreshToken);
        }
    }

    private Error FormatValidationErrors(
        List<ValidationFailure> failures)
    {
        string aggregatedErrors = string.Join(
            " | ",
            failures.Select(
                f => $"{f.PropertyName}: {f.ErrorMessage}"));

        return Error.Validation(
            "Model.Validation",
            $"Input validation failed: {aggregatedErrors}");
    }
}