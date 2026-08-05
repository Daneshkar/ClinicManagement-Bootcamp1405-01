using System;
using System.Collections.Generic;
using System.Text;
using ClinicManagement.Application.DTOs.Patients;
using FluentValidation;

namespace ClinicManagement.Application.Validators
{
    public class PatientSignupRequestValidator : AbstractValidator<PatientSignupRequest>
    {


        public PatientSignupRequestValidator()
        {
            RuleFor(x => x.NationalCode)
        .NotEmpty()
        .WithMessage("National Code is required.")
        .Length(10)
        .WithMessage("National Code must be 10 characters.");


            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(30)
                .WithMessage("must not exceed 30 characters");


            RuleFor(x => x.Phone)
             .Length(11)
             .WithMessage("Phone number must be 11 characters.")
             .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.");

        }
    }

    public class UpdateRequestValidator : AbstractValidator<UpdatePatientRequest>
    {

        public UpdateRequestValidator()
        {

            RuleFor(x => x.NationalCode)
     .NotEmpty()
     .WithMessage("National Code is required.")
     .Length(10)
     .WithMessage("National Code must be 10 characters.");



            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(30)
                .WithMessage("must not exceed 30 characters");


            RuleFor(x => x.Phone)
            .Length(11)
            .WithMessage("Phone number must be 11 characters.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        }
        public class DeletePatientRequestValidator : AbstractValidator<DeletePatientRequest>
        {



            public DeletePatientRequestValidator()
            {
                RuleFor(x => x.NationalCode)
         .NotEmpty()
         .WithMessage("National Code is required.");


            }



        }
    }
}