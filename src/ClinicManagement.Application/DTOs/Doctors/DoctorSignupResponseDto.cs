using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.DTOs.Doctors
{
    public record DoctorSignupResponseDto

    {
        public string MedicalID { get; set; }
        public string Name { get; set; }
        public decimal Fee { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }

    }
}
