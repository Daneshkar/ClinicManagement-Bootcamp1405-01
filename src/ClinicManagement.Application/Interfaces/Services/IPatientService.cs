
using System.Collections.Generic;
using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Patients;

namespace ClinicManagement.Application.Interfaces.Services
{
    public interface IPatientService
    {
        Task<Result<PatientResponse>> SignupAsync(
         PatientSignupRequest request);

        Task<Result<IEnumerable<PatientResponse>>> GetAllPatientsAsync(
      GetAllPatientsRequest request);

        Task<Result<PatientResponse>> GetPatientByNationalCodeAsync(GetPatientByNationalCodeRequest request);
           

        Task<Result<PatientResponse>> UpdatePatientAsync(
            
            UpdatePatientRequest request);

        Task<Result<PatientResponse>> DeletePatientAsync(
            DeletePatientRequest request);
    }
}
