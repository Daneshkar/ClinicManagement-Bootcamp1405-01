using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.DTOs.Doctors
{
    public record DoctorSignupRequest(
    string MedicalId,
    string Name,
    string Password,
    decimal Fee)
    {
        public string MedicalId { get; init; } = MedicalId?.Trim() ?? "";
        public string Name { get; init; } = Name?.Trim() ?? "";
    }

    public record GetDoctorByIdRequest(string MedicalId)
    {
        public string MedicalId { get; init; } = MedicalId?.Trim() ?? "";
    }

    public record GetAllDoctorsRequest();

    public record UpdateDoctorRequest(
    string MedicalId,
    string Name,
    decimal Fee)
    {
        public string MedicalId { get; init; } = MedicalId?.Trim() ?? "";
        public string Name { get; init; } = Name?.Trim() ?? "";
    }


    public record DeleteDoctorRequest(string MedicalId)
    {
        public string MedicalId { get; init; } = MedicalId?.Trim() ?? "";
    }

    public record DoctorResponse(
    string MedicalId,
    string Name,
    decimal Fee);
}
