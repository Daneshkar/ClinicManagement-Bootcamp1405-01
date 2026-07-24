namespace ClinicManagement.Application.DTOs.Doctors;

public record DoctorUpdateRequestDto(
    string Name,
    decimal Fee
);