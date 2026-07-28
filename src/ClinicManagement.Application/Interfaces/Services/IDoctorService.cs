using ClinicManagement.Application.DTOs.Doctors;

namespace ClinicManagement.Application.Interfaces.Services;

public interface IDoctorService
{
    Task<DoctorSignupResponseDto> SignupAsync(DoctorSignupRequestDto request);



    Task<IEnumerable<DoctorGetResponseDto>> GetAllDoctorsAsync();


    Task<DoctorGetResponseDto?> GetDoctorByMedicalIdAsync(string medicalId);
    

    Task<DoctorUpdateResponseDto> UpdateDoctorAsync(string medicalId, DoctorUpdateRequestDto request);

    Task<DoctorDeleteResponseDto> DeleteDoctorAsync(string medicalId);



}