using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Reports;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using FluentValidation;

namespace ClinicManagement.Application.Services
{
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
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<Result<FinancialReportResponse>> GetFinancialReportAsync(
            GetFinancialReportRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate request DTO
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<FinancialReportResponse>.Failure(
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // 2. Validate existence of each DoctorMedicalId against IDoctorRepository
            var distinctDoctorIds = request.DoctorMedicalIds.Distinct().ToList();
            var existingDoctors = new List<Doctor>();

            foreach (var medicalId in distinctDoctorIds)
            {
                var doctor = await _doctorRepository.GetByMedicalIdAsync(medicalId);
                if (doctor is null)
                {
                    return Result<FinancialReportResponse>.Failure(
                        Error.NotFound($"Doctor with Medical ID '{medicalId}' was not found."));
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

            // Group appointments by doctor
            var appointmentsByDoctor = visitedAppointments
                .GroupBy(a => a.DoctorMedicalId)
                .ToDictionary(g => g.Key, g => g.Count());

            // 5. Calculate individual doctor revenues and aggregate GrandTotal
            var doctorReports = new List<DoctorFinancialReportDto>();
            decimal grandTotal = 0m;

            foreach (var doctor in existingDoctors)
            {
                appointmentsByDoctor.TryGetValue(doctor.MedicalId, out var count);
                var revenue = count * doctor.Fee;
                grandTotal += revenue;

                doctorReports.Add(new DoctorFinancialReportDto(
                    doctor.MedicalId,
                    doctor.Name,
                    doctor.Fee,
                    count,
                    revenue));
            }

            // 6. Return wrapped Result<FinancialReportResponse>
            var response = new FinancialReportResponse(
                request.Period,
                fromDate,
                toDate,
                doctorReports,
                grandTotal);

            return Result<FinancialReportResponse>.Success(response);
        }

        private static (DateTime? FromDate, DateTime? ToDate) GetCalendarBoundaries(GetFinancialReportRequest request)
        {
            var today = DateTime.UtcNow.Date;

            switch (request.Period)
            {
                case TimePeriodOption.AllTime:
                    return (null, null);

                case TimePeriodOption.LastDay:
                    return (today.AddDays(-1), EndOfDay(today));

                case TimePeriodOption.LastWeek:
                    return (today.AddDays(-7), EndOfDay(today));

                case TimePeriodOption.LastMonth:
                    return (today.AddMonths(-1), EndOfDay(today));

                case TimePeriodOption.Custom:
                    var from = request.CustomFromDate!.Value.Date;
                    var to = request.CustomToDate!.Value.Date;
                    return (from, EndOfDay(to));

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(request.Period),
                        request.Period,
                        "Unsupported TimePeriodOption value.");
            }
        }

        private static DateTime EndOfDay(DateTime date) => date.Date.AddDays(1).AddTicks(-1);
    }
}