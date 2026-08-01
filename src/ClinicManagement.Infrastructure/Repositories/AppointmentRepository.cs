using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ClinicDbContext _context;

        public AppointmentRepository(ClinicDbContext context)
        {
            _context = context;
        }

        public async Task<List<DateTime>> GetBookedVisitDatesAsync(string doctorMedicalId, DateTime date)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Where(a => a.DoctorMedicalId == doctorMedicalId && a.VisitDate.Date == date.Date)
                .Select(a => a.VisitDate)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string doctorMedicalId, DateTime visitDate)
        {
            return await _context.Appointments
                .AnyAsync(a => a.DoctorMedicalId == doctorMedicalId && a.VisitDate == visitDate);
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }
    }
}