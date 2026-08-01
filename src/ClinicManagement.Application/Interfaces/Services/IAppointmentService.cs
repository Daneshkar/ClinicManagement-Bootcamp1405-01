using ClinicManagement.Application.DTOs.Appointments;

namespace ClinicManagement.Application.Interfaces.Services
{
    public interface IAppointmentService
    {
        public Task<DoctorAvailableSlotsResponseDto> GetAvailableSlotsAsync(string doctorMedicalId, DateTime date);
        public Task<AppointmentCreateResponseDto> BookAppointmentAsync(AppointmentCreateRequestDto appointmentCreateRequestDto);
    }
}
