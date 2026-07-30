using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.DTOs.Patients;

public record PatientUpdateResponseDto(
    bool IsSuccess,
    string? Message
);