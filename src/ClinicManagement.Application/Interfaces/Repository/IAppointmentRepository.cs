using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Application.Interfaces.Repository
{
    public interface IAppointmentRepository
    {
        public Task<IEnumerable<Appointment>> GetBookedVisitDatesAsync(string medicalId, DateTime startDate, DateTime endDate);
        public Task<bool> ExistsAsync(string medicalId, DateTime startDate);
        public Task AddAsync(Appointment appointment);
        public Task<IEnumerable<Appointment>> GetVisitedAppointmentsForFinancialReportAsync(
         List<string> doctorMedicalIds,
         DateTime? fromDate,
         DateTime? toDate);

        Task<IEnumerable<Appointment>> GetTodayAppointmentsByDoctorIdAsync(
        string doctorMedicalId,
         DateTime date);

        Task<Appointment?> GetByIdAsync(Guid id);

        Task UpdateAsync(Appointment appointment);

        Task SaveChangesAsync();
    }
}
