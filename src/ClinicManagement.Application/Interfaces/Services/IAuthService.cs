using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<LoginServiceResult>> LoginAsync(LoginRequest request);
        public  Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken);

        public  Task RevokeRefreshTokenAsync(string refreshToken);

    }
}
