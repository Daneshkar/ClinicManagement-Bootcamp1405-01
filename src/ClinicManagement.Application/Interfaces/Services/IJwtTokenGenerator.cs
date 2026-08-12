using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Interfaces.Services
{
    public interface IJwtTokenGenerator
    {

        public string GenerateAccessToken(Doctor  doctor);
        public string GenerateRefreshToken();


    }
}
