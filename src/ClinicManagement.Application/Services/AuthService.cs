using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Auth;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Domain.Entities;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IValidator<DoctorLoginRequest> _loginValidator;
        public AuthService(IDoctorRepository doctorRepository,

           IRefreshTokenRepository refreshTokenRepository,
            IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator, IValidator<DoctorLoginRequest> loginValidator)
        {
            _doctorRepository = doctorRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _loginValidator = loginValidator;

        }
        public async Task<Result<LoginServiceResult>> LoginAsync(DoctorLoginRequest request)
        {
            var validationResult = await _loginValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var error = FormatValidationErrors(validationResult.Errors);
                return error;
            }
            var doctor = await _doctorRepository.GetByMedicalIdAsync(request.MedicalId);
            if (doctor is null)
            {
                return Error.Unauthorized("Auth.InvalidCredentials",
                "Invalid medical ID or password.");
            }
            if (!_passwordHasher.IsMatch(request.Password, doctor.PasswordHash))
            {
                return Error.Unauthorized("Auth.InvalidCredentials",
               "Invalid medical ID or password.");

            }
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(doctor);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
            var newRefreshToken = RefreshToken.Create(doctor.MedicalId, refreshToken,
    TimeSpan.FromDays(7));
       await _refreshTokenRepository.AddAsync(newRefreshToken);
            var result = new LoginServiceResult(accessToken,refreshToken,
                newRefreshToken.ExpiresAt,doctor.MedicalId);
            return result;
        }

        public async Task<Result<AuthResponse>> RefreshTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return Error.Unauthorized(
                    "Auth.InvalidToken",
                    "Invalid or expired refresh token.");
            }
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
            
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.IsUsed || refreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                return Error.Unauthorized("Auth.InvalidToken","Auth.InvalidRefreshToken"); 
             
            }
            var doctor=await _doctorRepository.GetByMedicalIdAsync(refreshToken.DoctorMedicalId);
            if (doctor is null)
            {
                return Error.Unauthorized("Auth.InvalidCredentials",
                "Invalid medical ID or password.");
            }
            refreshToken.MarkAsUsed();
            await _refreshTokenRepository.UpdateAsync(refreshToken);
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(doctor);
            var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();
            var newRefreshToken = RefreshToken.Create(doctor.MedicalId, refreshTokenValue, TimeSpan.FromDays(7));
            await _refreshTokenRepository.AddAsync(newRefreshToken);
            var result = new AuthResponse(accessToken, refreshTokenValue);
            return result;
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
        
        private Error FormatValidationErrors(List<ValidationFailure> failures)
        {
            string aggregatedErrors = string.Join(" | ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
            // Using ErrorType.Validation and a generic code for all validation failures
            return Error.Validation(
                "Model.Validation", // A generic code for validation issues
                $"Input validation failed: {aggregatedErrors}"
            );
        }
        
    }
}
