using System;
using System.Collections.Generic;
using System.Text;
using ClinicManagement.Application.DTOs.Patients;

namespace ClinicManagement.Application.Interfaces.Services
{
    public interface IPatientService
    {
        Task<PatientCreateResponseDto> CreatePatientAsync(
        PatientCreateRequestDto request);

        Task<IEnumerable<PatientGetResponseDto>> GetAllPatientsAsync();

        Task<PatientGetResponseDto?> GetPatientByNationalCodeAsync(
            string nationalCode);

        Task<PatientUpdateResponseDto> UpdatePatientAsync(
            string nationalCode,
            PatientUpdateRequestDto request);

        Task<PatientDeleteResponseDto> DeletePatientAsync(
            string nationalCode);
    }
}
