using ClinicManagement.Application.DTOs.Doctors;

namespace ClinicManagement.Application.Interfaces.Services;

public interface IDoctorService
{
    public Task<DoctorSignupResponseDto> SignupAsync(DoctorSignupRequestDto request);



    public Task<IEnumerable<DoctorGetResponseDto>> GetAllDoctorsAsync();


    public Task<DoctorGetResponseDto> GetDoctorByMedicalIdAsync(string medicalId);
    

    public  Task<DoctorUpdateResponseDto> UpdateDoctorAsync(string medicalId, DoctorUpdateRequestDto request);

    public  Task<DoctorDeleteResponseDto> DeleteDoctorAsync(string medicalId);



}