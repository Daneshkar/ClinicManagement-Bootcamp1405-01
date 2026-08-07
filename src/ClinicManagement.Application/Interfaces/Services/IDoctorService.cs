using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Doctors;

namespace ClinicManagement.Application.Interfaces.Services;


public interface IDoctorService
{
    Task<Result<DoctorResponse>> SignupAsync(DoctorSignupRequest request);

    Task<Result<IEnumerable<DoctorResponse>>> GetAllDoctorsAsync(GetAllDoctorsRequest request);

    Task<Result<DoctorResponse>> GetDoctorByMedicalIdAsync(GetDoctorByIdRequest request);

    Task<Result<DoctorResponse>> UpdateDoctorAsync(UpdateDoctorRequest request);

    Task<Result<DoctorResponse>> DeleteDoctorAsync(DeleteDoctorRequest request);
}


