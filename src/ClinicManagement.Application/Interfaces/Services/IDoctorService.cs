using ClinicManagement.Application.DTOs.Doctors;

namespace ClinicManagement.Application.Interfaces.Services;

public interface IDoctorService
{
    public Task<DoctorSignupResponseDto> SignupAsync(DoctorSignupRequestDto request);
}