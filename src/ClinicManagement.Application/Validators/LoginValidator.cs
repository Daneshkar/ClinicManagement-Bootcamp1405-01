using ClinicManagement.Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Validators
{
    class LoginValidator
        : AbstractValidator<DoctorLoginRequest>
    {
        public  LoginValidator() {

            RuleFor(x => x.MedicalId)
                   .NotEmpty()
                   .WithMessage("Medical ID is required.")
                   .MaximumLength(20)
                   .WithMessage("Medical ID cannot exceed 20 characters.");

            RuleFor(x => x.Password)
              .NotEmpty()
              .WithMessage("Password is required.")
              .MinimumLength(8)
              .WithMessage("Password must be at least 8 characters.");


        }



    }
}
