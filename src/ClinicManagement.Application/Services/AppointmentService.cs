using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Appointments;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace ClinicManagement.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IValidator<GetDoctorAvailableSlotsRequest> _getDoctorAvailableSlotsRequestValidator;
    private readonly IValidator<AppointmentCreateRequest> _appointmentCreateRequestValidator;

    private const int StartOperatingHour = 9;
    private const int EndOperatingHour = 15;
    private const int ScheduleDaysSpan = 7;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IValidator<GetDoctorAvailableSlotsRequest> getDoctorAvailableSlotsRequestValidator,
        IValidator<AppointmentCreateRequest> appointmentCreateRequestValidator)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _getDoctorAvailableSlotsRequestValidator = getDoctorAvailableSlotsRequestValidator;
        _appointmentCreateRequestValidator = appointmentCreateRequestValidator;
    }

    public async Task<Result<DoctorAvailableSlotsResponse>> GetAvailableSlotsAsync(GetDoctorAvailableSlotsRequest request)
    {
        // 1. FluentValidation check
        var validationResult = await _getDoctorAvailableSlotsRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return FormatValidationErrors(validationResult.Errors);
        }

        // 2. DB State Check: Doctor Existence
        var doctorExists = await _doctorRepository.ExistsByMedicalIdAsync(request.DoctorMedicalId);
        if (!doctorExists)
        {
            return Error.NotFound(
                "Doctor.NotFound",
                $"Doctor with Medical ID '{request.DoctorMedicalId}' was not found.");
        }

        // 3. Time Handling: Use Local Time for Clinic Operating Hours
        var now = DateTime.Now; // Ensure server timezone matches clinic timezone, or use TimeZoneInfo
        var rangeStart = now.Date;
        var rangeEnd = rangeStart.AddDays(ScheduleDaysSpan);

        List<DateTime> allPossibleSlots = GenerateWorkingHoursForNextWeek(rangeStart);
        var bookedAppointments = await _appointmentRepository.GetBookedVisitDatesAsync(request.DoctorMedicalId, rangeStart, rangeEnd);
        var bookedSet = new HashSet<DateTime>(bookedAppointments.Select(a => a.VisitDate));

        // Filter out past slots (slot must be in the future relative to current exact time)
        var freeSlots = allPossibleSlots
            .Where(slot => slot > now && !bookedSet.Contains(slot))
            .Select(slot => new AvailableSlotResponse(
                StartTime: slot,
                EndTime: slot.AddMinutes(59).AddSeconds(59)))
            .ToList();

        return new DoctorAvailableSlotsResponse(request.DoctorMedicalId, freeSlots);
    }

    public async Task<Result<AppointmentResponse>> BookAppointmentAsync(AppointmentCreateRequest request)
    {
        // 1. FluentValidation check
        var validationResult = await _appointmentCreateRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return FormatValidationErrors(validationResult.Errors);
        }

        // 2. Prevent booking past dates/times
        if (request.VisitDate <= DateTime.Now)
        {
            return Error.Validation(
                "Appointment.PastDate",
                "Cannot book an appointment for a past date or time.");
        }

        // 3. DB State Check: Doctor Existence
        var doctorExists = await _doctorRepository.ExistsByMedicalIdAsync(request.DoctorMedicalId);
        if (!doctorExists)
        {
            return Error.NotFound(
                "Doctor.NotFound",
                $"Doctor with Medical ID '{request.DoctorMedicalId}' was not found.");
        }

        // 4. DB State Check: Patient Existence
        var patientExists = await _patientRepository.ExistsByNationalCodeAsync(request.PatientNationalCode);
        if (!patientExists)
        {
            return Error.NotFound(
                "Patient.NotFound",
                $"Patient with National Code '{request.PatientNationalCode}' was not found.");
        }

        // 5. DB State Check: Reserved Appointment Check
        var appointmentIsReserved = await _appointmentRepository.ExistsAsync(request.DoctorMedicalId, request.VisitDate);
        if (appointmentIsReserved)
        {
            return Error.Conflict(
                "Appointment.AlreadyReserved",
                "The requested appointment slot is already reserved.");
        }

        // 6. Create & Save
        var appointment = Appointment.Create(
            doctorMedicalId: request.DoctorMedicalId,
            patientNationalCode: request.PatientNationalCode,
            visitDate: request.VisitDate
        );

        await _appointmentRepository.AddAsync(appointment);

        return new AppointmentResponse(
            appointment.DoctorMedicalId,
            appointment.PatientNationalCode,
            appointment.VisitDate);
    }

    private static List<DateTime> GenerateWorkingHoursForNextWeek(DateTime startDate)
    {
        List<DateTime> slotList = new List<DateTime>();

        for (int currentDay = 0; currentDay < ScheduleDaysSpan; currentDay++)
        {
            for (int currentHour = StartOperatingHour; currentHour < EndOperatingHour; currentHour++)
            {
                slotList.Add(startDate.Date.AddDays(currentDay).AddHours(currentHour));
            }
        }

        return slotList;
    }

    private Error FormatValidationErrors(List<ValidationFailure> failures)
    {
        string aggregatedErrors = string.Join(" | ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
        return Error.Validation(
            "Model.Validation",
            $"Input validation failed: {aggregatedErrors}"
        );
    }
}