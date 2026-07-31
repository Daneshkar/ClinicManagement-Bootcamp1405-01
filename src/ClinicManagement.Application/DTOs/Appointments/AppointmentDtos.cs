namespace ClinicManagement.Application.DTOs.Appointments;

public record AvailableSlotDto(
    DateTime StartTime,
    DateTime EndTime
);

public record DoctorAvailableSlotsResponseDto(
    string DoctorMedicalId,
    IEnumerable<AvailableSlotDto> AvailableSlots
);

public record AppointmentCreateRequestDto(
    string DoctorMedicalId,
    string PatientNationalCode,
    DateTime VisitDate
);

public record AppointmentCreateResponseDto(
    bool IsSuccess,
    string? Message,
    DateTime? VisitDate
);