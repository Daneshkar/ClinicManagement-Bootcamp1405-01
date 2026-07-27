using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.DTOs.Patients;

public record PatientGetResponseDto(
    string NationalCode,
    string Name,
    string Phone
);
