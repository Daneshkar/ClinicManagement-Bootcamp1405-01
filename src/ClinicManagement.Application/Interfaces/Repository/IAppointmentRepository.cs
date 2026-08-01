using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Application.Interfaces.Repository
{
    public interface IAppointmentRepository
    {
        public Task<List<DateTime>> GetBookedVisitDatesAsync(string doctorMedicalId, DateTime date);
        public Task<bool> ExistsAsync(string doctorMedicalId, DateTime visitDate);
        public Task AddAsync(Appointment appointment);
    }
}
