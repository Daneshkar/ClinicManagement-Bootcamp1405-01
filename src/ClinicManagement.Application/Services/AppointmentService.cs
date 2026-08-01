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
    private const int ScheduleDaysSpan  = 7;

    public AppointmentService(IAppointmentRepository appointmentRepository, IDoctorRepository doctorRepository,
        IPatientRepository patientRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
    }
    
    public async Task<DoctorAvailableSlotsResponseDto> GetAvailableSlotsAsync(string medicalId, DateTime startDate)
    {
        var doctorExists = await _doctorRepository.ExistsByMedicalIdAsync(medicalId);
        if (!doctorExists)
        {
            return null;
        }

        // Generate 7 full calendar days starting from 00:00 today
        var rangeStart = startDate.Date;
        var rangeEnd = rangeStart.AddDays(ScheduleDaysSpan);

        List<DateTime> allPossibleSlots = GenerateWorkingHoursForNextWeek(rangeStart);
        var bookedAppointments = await _appointmentRepository.GetBookedVisitDatesAsync(medicalId, rangeStart, rangeEnd);
        var bookedSet = new HashSet<DateTime>(bookedAppointments.Select(a => a.VisitDate));

        // Filter out:
        // 1) Slots earlier than the requested time (e.g. 09:00, 10:00, 11:00 if startDate is 12:12)
        // 2) Slots already reserved
        var freeSlots = allPossibleSlots
            .Where(slot => slot > startDate && !bookedSet.Contains(slot))
            .Select(slot => new AvailableSlotDto(
                StartTime: slot,
                EndTime: slot.AddMinutes(59).AddSeconds(59)))
            .ToList();

        return new DoctorAvailableSlotsResponseDto(medicalId, freeSlots);
    }
    public async Task<AppointmentCreateResponseDto> BookAppointmentAsync(AppointmentCreateRequestDto request)
    {
        // check for patient existance
        var doctorExists = await _patientRepository.ExistsByNationalCodeAsync(request.PatientNationalCode);
        if (!doctorExists)
        {
            return  new AppointmentCreateResponseDto(
                IsSuccess: false,
                Message: "patient doesnt exist",
                null
            );
        }
        
        var appointmentIsReserved = await _appointmentRepository.ExistsAsync(request.DoctorMedicalId, request.VisitDate);
        if (appointmentIsReserved)
        {
            return new AppointmentCreateResponseDto(
                IsSuccess: false,
                Message: "appointment is already reserved",
                null);
        }

        var appointment = Appointment.Create(
            doctorMedicalId: request.DoctorMedicalId,
            patientNationalCode: request.PatientNationalCode,
            visitDate: request.VisitDate
        );
        await _appointmentRepository.AddAsync(appointment);
        return new AppointmentCreateResponseDto(IsSuccess: true, Message: "appointment reserved successfully", VisitDate: request.VisitDate);
    }


    // helper method to generate a list of all possible slots across 7-day lookahead each day from 9am till 15pm
    // output list should contain 6 * 7 = 42, dateTime objects!
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
    

}
}