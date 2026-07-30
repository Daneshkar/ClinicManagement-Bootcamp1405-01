using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.DTOs.Patients;

public record PatientCreateRequestDto(
    string NationalCode,
    string Name,
    string Phone
);

