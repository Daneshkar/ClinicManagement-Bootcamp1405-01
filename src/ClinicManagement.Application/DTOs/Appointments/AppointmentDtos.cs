using System;
using System.Collections.Generic;

namespace ClinicManagement.Application.DTOs.Appointments;

public record GetDoctorAvailableSlotsRequest(
    string DoctorMedicalId)
{
    public string DoctorMedicalId { get; init; } = DoctorMedicalId?.Trim() ?? string.Empty;
}

public record AvailableSlotResponse(
    DateTime StartTime,
    DateTime EndTime);

public record DoctorAvailableSlotsResponse(
    string DoctorMedicalId,
    IEnumerable<AvailableSlotResponse> AvailableSlots);

public record AppointmentCreateRequest(
    string DoctorMedicalId,
    string PatientNationalCode,
    DateTime VisitDate)
{
    public string DoctorMedicalId { get; init; } = DoctorMedicalId?.Trim() ?? string.Empty;
    public string PatientNationalCode { get; init; } = PatientNationalCode?.Trim() ?? string.Empty;
    public DateTime VisitDate { get; init; } = NormalizeToNextHour(VisitDate);

    private static DateTime NormalizeToNextHour(DateTime dt)
    {
        return (dt.Minute != 0 || dt.Second != 0 || dt.Millisecond != 0)
            ? new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0, dt.Kind).AddHours(1)
            : dt;
    }
}

public record AppointmentResponse(
    string DoctorMedicalId,
    string PatientNationalCode,
    DateTime VisitDate);