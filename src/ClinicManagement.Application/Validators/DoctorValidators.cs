using ClinicManagement.Application.DTOs.Doctors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Validators
{
    public class DoctorSignupRequestValidator
    : AbstractValidator<DoctorSignupRequest>
    {
        public DoctorSignupRequestValidator()
        {
            RuleFor(x => x.MedicalId)
                .NotEmpty()
                .WithMessage("Medical ID is required.")
                .MaximumLength(20)
                .WithMessage("Medical ID cannot exceed 20 characters.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Doctor name is required.")
                .MaximumLength(30)
                .WithMessage("Doctor name cannot exceed 30 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.");

            RuleFor(x => x.Fee)
                .GreaterThan(0)
                .WithMessage("Consultation fee must be greater than zero.");
        }
    }


    public class GetDoctorByIdRequestValidator
        : AbstractValidator<GetDoctorByIdRequest>
    {
        public GetDoctorByIdRequestValidator()
        {
            RuleFor(x => x.MedicalId)
                .NotEmpty()
                .WithMessage("Medical ID is required.")
                .MaximumLength(20)
                .WithMessage("Medical ID cannot exceed 20 characters.");
        }
    }


    public class UpdateDoctorRequestValidator
        : AbstractValidator<UpdateDoctorRequest>
    {
        public UpdateDoctorRequestValidator()
        {
            RuleFor(x => x.MedicalId)
                .NotEmpty()
                .WithMessage("Medical ID is required.")
                .MaximumLength(20)
                .WithMessage("Medical ID cannot exceed 20 characters.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Doctor name is required.")
                .MaximumLength(30)
                .WithMessage("Doctor name cannot exceed 30 characters.");

            RuleFor(x => x.Fee)
                .GreaterThan(0)
                .WithMessage("Consultation fee must be greater than zero.");
        }
    }


    public class DeleteDoctorRequestValidator
        : AbstractValidator<DeleteDoctorRequest>
    {
        public DeleteDoctorRequestValidator()
        {
            RuleFor(x => x.MedicalId)
                .NotEmpty()
                .WithMessage("Medical ID is required for deletion.");
        }
    }
}
