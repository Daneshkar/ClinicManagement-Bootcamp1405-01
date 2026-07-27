using ClinicManagement.Application.DTOs.Patients;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Interfaces
{
    public interface IPatientService
    {
        public Task<PatientCreateResponseDto> CreatePatientAsync(
        PatientCreateRequestDto request);

        public Task<IEnumerable<PatientGetResponseDto>> GetAllPatientsAsync();

        public Task<PatientGetResponseDto?> GetPatientByNationalCodeAsync(
            string nationalCode);

        public Task<PatientUpdateResponseDto> UpdatePatientAsync(
            string nationalCode,
            PatientUpdateRequestDto request);

        public Task<PatientDeleteResponseDto> DeletePatientAsync(
            string nationalCode);
    }
}
