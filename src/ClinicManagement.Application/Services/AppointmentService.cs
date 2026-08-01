using ClinicManagement.Application.DTOs.Appointments;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;

        private const int StartOperatingHour = 9;
        private const int EndOperatingHour = 15;
        private const int ScheduleDaysSpan = 7;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository)
        {
            _appointmentRepository = appointmentRepository;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
        }

        private static List<DateTime> GenerateWorkingHoursForNextWeek(DateTime startDate)
        {
            var workingHours = new List<DateTime>();
            var baseDate = startDate.Date;

            for (int day = 0; day < ScheduleDaysSpan; day++)
            {
                var currentDay = baseDate.AddDays(day);

                for (int hour = StartOperatingHour; hour < EndOperatingHour; hour++)
                {
                    workingHours.Add(currentDay.AddHours(hour));
                }
            }

            return workingHours;
        }

        public async Task<DoctorAvailableSlotsResponseDto> GetAvailableSlotsAsync(string doctorMedicalId, DateTime startDate)
        {
            var doctorExists = await _doctorRepository.ExistsByMedicalIdAsync(doctorMedicalId);
            if (!doctorExists)
            {
                return new DoctorAvailableSlotsResponseDto(doctorMedicalId, new List<AvailableSlotDto>());
            }

            var start = startDate.Date;

            var workingHours = GenerateWorkingHoursForNextWeek(start);

            var bookedSet = new HashSet<DateTime>();
            for (int day = 0; day < ScheduleDaysSpan; day++)
            {
                var currentDay = start.AddDays(day);
                var bookedVisitDatesForDay = await _appointmentRepository.GetBookedVisitDatesAsync(doctorMedicalId, currentDay);

                foreach (var bookedDate in bookedVisitDatesForDay)
                {
                    bookedSet.Add(bookedDate);
                }
            }

            var availableSlots = workingHours
                .Where(slot => !bookedSet.Contains(slot))
                .Select(slot => new AvailableSlotDto(slot, slot.AddHours(1)))
                .ToList();

            return new DoctorAvailableSlotsResponseDto(doctorMedicalId, availableSlots);
        }

        public async Task<AppointmentCreateResponseDto> BookAppointmentAsync(AppointmentCreateRequestDto request)
        {
            var doctorExists = await _doctorRepository.ExistsByMedicalIdAsync(request.DoctorMedicalId);
            if (!doctorExists)
            {
                return new AppointmentCreateResponseDto(false, "The specified doctor was not found.", null);
            }

            var patientExists = await _patientRepository.ExistsByNationalCodeAsync(request.PatientNationalCode);
            if (!patientExists)
            {
                return new AppointmentCreateResponseDto(false, "The specified patient was not found.", null);
            }

            var slotTaken = await _appointmentRepository.ExistsAsync(request.DoctorMedicalId, request.VisitDate);
            if (slotTaken)
            {
                return new AppointmentCreateResponseDto(false, "The requested time slot is already booked for this doctor.", null);
            }

            var appointment = Appointment.Create(request.DoctorMedicalId, request.PatientNationalCode, request.VisitDate);

            await _appointmentRepository.AddAsync(appointment);

            return new AppointmentCreateResponseDto(true, "Appointment booked successfully.", appointment.VisitDate);
        }
    }
}