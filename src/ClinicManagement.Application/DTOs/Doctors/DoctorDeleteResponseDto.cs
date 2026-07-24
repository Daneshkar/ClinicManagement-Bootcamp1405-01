namespace ClinicManagement.Application.DTOs.Doctors;

public record DoctorDeleteResponseDto(
    bool IsSuccess,
    string? Message = null
);