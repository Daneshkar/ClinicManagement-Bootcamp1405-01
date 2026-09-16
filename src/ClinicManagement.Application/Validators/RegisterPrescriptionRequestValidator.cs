using ClinicManagement.Application.DTOs.Treatment;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Validators;

public class RegisterPrescriptionRequestValidator : AbstractValidator<RegisterPrescriptionRequest>
{
    public RegisterPrescriptionRequestValidator()
    {
        RuleFor(x => x.AppointmentId)
            .NotEmpty()
            .WithMessage("Appointment ID is required.");

        RuleFor(x => x.Prescription)
            .NotEmpty()
            .WithMessage("Prescription notes cannot be empty.")
            .MaximumLength(1000)
            .WithMessage("Prescription cannot exceed 1000 characters.");
    }
}