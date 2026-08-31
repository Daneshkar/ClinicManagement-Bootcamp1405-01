using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Treatment;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Domain.Enums;
using FluentValidation;
using FluentValidation.Results;


namespace ClinicManagement.Application.Services
{
    public class TreatmentService : ITreatmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IValidator<RegisterPrescriptionRequest> _prescriptionValidator;

        public TreatmentService(
            IAppointmentRepository appointmentRepository,
            IValidator<RegisterPrescriptionRequest> prescriptionValidator)
        {
            _appointmentRepository = appointmentRepository;
            _prescriptionValidator = prescriptionValidator;
        }

        public async Task<Result<IEnumerable<TodayAppointmentResponse>>> GetTodayAppointmentsAsync(
            string doctorMedicalId,
            DateTime currentDate)
        {
            var appointments = await _appointmentRepository.GetTodayAppointmentsByDoctorIdAsync(
                doctorMedicalId,
                currentDate.Date);

            var statusUpdated = false;

            foreach (var appointment in appointments)
            {
                if (appointment.Status == AppointmentStatus.Reserved &&
                    currentDate > appointment.VisitDate.AddMinutes(59))
                {
                    appointment.Status = AppointmentStatus.Missed;
                    statusUpdated = true;
                }
            }

            if (statusUpdated)
            {
                await _appointmentRepository.SaveChangesAsync();
            }
        var response = appointments.Select(a => new TodayAppointmentResponse(
                                            a.Id,
                                            a.PatientNationalCode,
                                            a.VisitDate,
                                            a.Status.ToString(),
                                            a.Prescription));

            return Result<IEnumerable<TodayAppointmentResponse>>.Success(response);
        }

        public async Task<Result> RegisterPrescriptionAsync(
            string doctorMedicalId,
            RegisterPrescriptionRequest request,
            DateTime currentTime)
        {
            ValidationResult validationResult = await _prescriptionValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result.Failure(Error.Validation("Treatment.Validation",errorMessage));            
            }

            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment is null)
            {
                return Error.NotFound("Appointment.NotFound", "Appointment not found.");
            }

            if (appointment.DoctorMedicalId != doctorMedicalId)
            {
                return Error.Forbidden("Treatment.Unauthorized", "You are not authorized to write prescriptions for this appointment.");            }

            var windowStart = appointment.VisitDate;
            var windowEnd = appointment.VisitDate.AddMinutes(59);

            if (currentTime < windowStart || currentTime > windowEnd)
            {
                return Error.Validation("Treatment.InvalidTimeWindow", "Prescriptions can only be recorded within the 59-minute appointment time window.");
            }
            
            if (appointment.Status == AppointmentStatus.Visited)
            {
                return Error.Conflict("Treatment.AlreadyVisited", "This appointment has already been completed.");
            }
            if (appointment.Status == AppointmentStatus.Missed)
            {
                return Error.Conflict("Treatment.AppointmentMissed", "Cannot register a prescription for a missed appointment.");
            }

            appointment.Prescription = request.Prescription;
            appointment.Status = AppointmentStatus.Visited;

            await _appointmentRepository.UpdateAsync(appointment);

            return Result.Success();
        }
    }
}