using ClinicManagement.Application.Interfaces.Repository;
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



    }
}
