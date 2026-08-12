using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Services
{
    public interface IAuthService
    {
        public  Task<Result<LoginServiceResult>> LoginAsync(DoctorLoginRequest request);

        public  Task<Result<AuthResponse>> RefreshTokenAsync(string refreshToken);

        public  Task RevokeRefreshTokenAsync(string refreshToken);

    }
}
