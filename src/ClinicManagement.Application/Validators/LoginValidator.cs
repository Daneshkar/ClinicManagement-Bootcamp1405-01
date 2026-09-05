using ClinicManagement.Application.DTOs.Auth;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;



namespace ClinicManagement.Application.Validators
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Identifier)
                .NotEmpty()
                .WithMessage("Identifier is required.")
                .MaximumLength(50)
                .WithMessage("Identifier cannot exceed 50 characters.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.");
        }
    }
}