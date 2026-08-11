using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Reports;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;

namespace ClinicManagement.Application.Services;

public class FinancialReportService : IFinancialReportService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IValidator<GetFinancialReportRequest> _validator;

    public FinancialReportService(
        IAppointmentRepository appointmentRepository,
        IDoctorRepository doctorRepository,
        IValidator<GetFinancialReportRequest> validator)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _validator = validator;
    }

    public async Task<Result<FinancialReportResponse>> GetFinancialReportAsync(GetFinancialReportRequest request)
    {
        // 1. Validate request DTO
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return FormatValidationErrors(validationResult.Errors);
        }

        // 2. Distinct IDs & Upfront Doctor Validation
        var distinctDoctorIds = request.DoctorMedicalIds.Distinct().ToList();
        var existingDoctors = new List<Doctor>();

        foreach (var medicalId in distinctDoctorIds)
        {
            var doctor = await _doctorRepository.GetByMedicalIdAsync(medicalId);
            if (doctor is null)
            {
                return Error.NotFound(
                    "Doctor.NotFound",
                    $"Doctor with Medical ID '{medicalId}' was not found.");
            }
            existingDoctors.Add(doctor);
        }

        // 3. Resolve strict calendar boundaries
        var (fromDate, toDate) = GetCalendarBoundaries(request);

        // 4. Fetch all matching Visited appointments
        var visitedAppointments = await _appointmentRepository.GetVisitedAppointmentsForFinancialReportAsync(
            distinctDoctorIds,
            fromDate,
            toDate);

        // 5. O(1) Dictionary Grouping for appointment counts
        var appointmentCountsByDoctor = visitedAppointments
            .GroupBy(a => a.DoctorMedicalId)
            .ToDictionary(g => g.Key, g => g.Count());

        // 6. Calculate revenues & Grand Total
        var doctorReports = new List<DoctorFinancialReportDto>();
        decimal grandTotal = 0m;

        foreach (var doctor in existingDoctors)
        {
            appointmentCountsByDoctor.TryGetValue(doctor.MedicalId, out var count);
            var revenue = count * doctor.Fee;
            grandTotal += revenue;

            doctorReports.Add(new DoctorFinancialReportDto(
                DoctorMedicalId: doctor.MedicalId,
                DoctorName: doctor.Name,
                Fee: doctor.Fee,
                VisitedAppointmentCount: count,
                Revenue: revenue));
        }

        // 7. Return Result
        return new FinancialReportResponse(
            Period: request.Period,
            FromDate: fromDate,
            ToDate: toDate,
            DoctorReports: doctorReports,
            GrandTotal: grandTotal);
    }

    /// <summary>
    /// Calculates strict calendar boundaries based on the requested period.
    /// </summary>
    private static (DateTime? FromDate, DateTime? ToDate) GetCalendarBoundaries(GetFinancialReportRequest request)
    {
        var today = DateTime.UtcNow.Date;

        return request.Period switch
        {
            TimePeriodOption.LastDay => (
                today.AddDays(-1),
                EndOfDay(today.AddDays(-1))
            ),
            TimePeriodOption.LastWeek => (
                today.AddDays(-7),
                EndOfDay(today)
            ),
            TimePeriodOption.LastMonth => (
                new DateTime(today.Year, today.Month, 1).AddMonths(-1),
                new DateTime(today.Year, today.Month, 1).AddTicks(-1)
            ),
            TimePeriodOption.Custom => (
                request.CustomFromDate!.Value.Date,
                EndOfDay(request.CustomToDate!.Value.Date)
            ),
            TimePeriodOption.AllTime => (null, null),
            _ => throw new ArgumentOutOfRangeException(nameof(request.Period), request.Period, "Unsupported period option.")
        };
    }

    private static DateTime EndOfDay(DateTime date) => date.Date.AddDays(1).AddTicks(-1);

    private Error FormatValidationErrors(List<ValidationFailure> failures)
    {
        string aggregatedErrors = string.Join(" | ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
        return Error.Validation(
            "Model.Validation",
            $"Input validation failed: {aggregatedErrors}"
        );
    }
}