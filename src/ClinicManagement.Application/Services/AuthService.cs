using ClinicManagement.Application.DTOs.Auth;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Services
{
    public class AuthService:IAuthService
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


       
            
    }
}
