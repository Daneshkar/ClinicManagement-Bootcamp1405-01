using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.DTOs.Patients;

public record PatientDeleteResponseDto(
    bool IsSuccess,
    string? Message
);