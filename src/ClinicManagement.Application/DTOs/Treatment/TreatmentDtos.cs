using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.DTOs.Treatment;

public record TodayAppointmentResponse(
    Guid Id,
    string PatientNationalCode,
    string PatientFullName,
    DateTime VisitDate,
    string Status,
    string? Prescription
);

public record RegisterPrescriptionRequest(
    Guid AppointmentId,
    string Prescription
);
