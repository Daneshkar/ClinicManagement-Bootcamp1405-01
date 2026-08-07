using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Appointments;

namespace ClinicManagement.Application.Interfaces.Services;

public interface IAppointmentService
{
    Task<Result<DoctorAvailableSlotsResponse>> GetAvailableSlotsAsync(GetDoctorAvailableSlotsRequest request);

    Task<Result<AppointmentResponse>> BookAppointmentAsync(AppointmentCreateRequest request);
}