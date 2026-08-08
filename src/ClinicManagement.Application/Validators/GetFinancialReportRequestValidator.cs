using ClinicManagement.Application.DTOs.Reports;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Application.Validators
{
    public class GetFinancialReportRequestValidator
    : AbstractValidator<GetFinancialReportRequest>
    {
        public GetFinancialReportRequestValidator()
        {
            RuleFor(x => x.DoctorMedicalIds)
                .NotNull()
                .WithMessage("Doctor medical IDs are required.")
                .Must(x => x != null && x.Count > 0)
                .WithMessage("At least one doctor medical ID is required.");

            RuleForEach(x => x.DoctorMedicalIds)
                .NotEmpty()
                .WithMessage("Doctor medical ID cannot be empty.")
                .MaximumLength(20)
                .WithMessage("Doctor medical ID cannot exceed 20 characters.");

            When(x => x.Period == TimePeriodOption.Custom, () =>
            {
                RuleFor(x => x.CustomFromDate)
                    .NotNull()
                     .WithMessage("Custom from date is required.");

                RuleFor(x => x.CustomToDate)
                    .NotNull()
                    .WithMessage("Custom to date is required.");
            });

            RuleFor(x => x)
                .Must(x =>
                    !x.CustomFromDate.HasValue ||
                    !x.CustomToDate.HasValue ||
                    x.CustomFromDate <= x.CustomToDate)
                .WithMessage("Custom from date must be less than or equal to custom to date.");
        }
    }
}
