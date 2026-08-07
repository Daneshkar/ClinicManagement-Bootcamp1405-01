using ClinicManagement.Application.DTOs.Appointments;
using FluentValidation;
using System;

namespace ClinicManagement.Application.Validators
{
    public class GetDoctorAvailableSlotsRequestValidator
        : AbstractValidator<GetDoctorAvailableSlotsRequest>
    {
        public GetDoctorAvailableSlotsRequestValidator()
        {
            RuleFor(x => x.DoctorMedicalId)
                .NotEmpty()
                .WithMessage("Medical ID is required.")
                .MaximumLength(20)
                .WithMessage("Medical ID cannot exceed 20 characters.");
        }
    }

    public class AppointmentCreateRequestValidator
        : AbstractValidator<AppointmentCreateRequest>
    {
        public AppointmentCreateRequestValidator()
        {
            RuleFor(x => x.DoctorMedicalId)
                .NotEmpty()
                .WithMessage("Medical ID is required.")
                .MaximumLength(20)
                .WithMessage("Medical ID cannot exceed 20 characters.");

            RuleFor(x => x.PatientNationalCode)
                .NotEmpty()
                .WithMessage("National Code is required.")
                .Length(10)
                .WithMessage("National Code must be 10 characters.");

            RuleFor(x => x.VisitDate)
                .NotEmpty()
                .WithMessage("Visit date and time are required.")
                .Must(x => x.TimeOfDay >= new TimeSpan(9, 0, 0) && x.TimeOfDay <= new TimeSpan(14, 0, 0))
                .WithMessage("Appointment time must be between 09:00 AM and 02:00 PM.");
        }
    }
}