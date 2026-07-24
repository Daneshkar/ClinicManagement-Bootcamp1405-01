namespace ClinicManagement.Application.DTOs.Doctors;

public record DoctorUpdateResponseDto(
    bool IsSuccess,
    string? Message = null
);