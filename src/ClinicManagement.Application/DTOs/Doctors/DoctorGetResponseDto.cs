namespace ClinicManagement.Application.DTOs.Doctors;

public record DoctorGetResponseDto(
    string MedicalID,
    string Name,
    decimal Fee
);
