using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Services
{
    public class AuthService:IAuthService
    {
        private readonly IDoctorRepository _doctorRepository;
        public AuthService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public AuthService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        private readonly IPasswordHasher _passwordHasher;
        public AuthService(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }


    }
}
