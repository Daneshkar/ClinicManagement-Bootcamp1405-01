using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories
{
    
    
        public class AppointmentRepository : IAppointmentRepository
        {
            private readonly ClinicDbContext clinicDbContext;

            public AppointmentRepository(ClinicDbContext context)
            {
                clinicDbContext = context;
            }

            public async Task<IEnumerable<Appointment>> GetBookedVisitDatesAsync(
                string medicalId,
                DateTime startDate,
                DateTime endDate)
            {
                var reservedSlots = clinicDbContext.Appointments
                    .Where(bookedList => bookedList.DoctorMedicalId == medicalId)
                    .Where(bookedSlot =>
                        bookedSlot.VisitDate >= startDate &&
                        bookedSlot.VisitDate < endDate)
                    .AsNoTracking()
                    .ToList();

                return reservedSlots;
            }

            public async Task<bool> ExistsAsync(
                string medicalId,
                DateTime startDate)
            {
                return await clinicDbContext.Appointments
                    .AnyAsync(a =>
                        a.DoctorMedicalId == medicalId &&
                        a.VisitDate == startDate);
            }

            public async Task AddAsync(Appointment appointment)
            {
                clinicDbContext.Appointments.Add(appointment);
                await clinicDbContext.SaveChangesAsync();
            }

            public async Task<IEnumerable<Appointment>> GetVisitedAppointmentsForFinancialReportAsync(
                List<string> doctorMedicalIds,
                DateTime? fromDate,
                DateTime? toDate)
            {
                var query = clinicDbContext.Appointments
                    .AsNoTracking()
                    .Where(a =>
                        a.Status == AppointmentStatus.Visited &&
                        doctorMedicalIds.Contains(a.DoctorMedicalId));

                if (fromDate.HasValue)
                {
                    query = query.Where(a =>
                        a.VisitDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(a =>
                        a.VisitDate <= toDate.Value);
                }

                return await query.ToListAsync();
            }

            public async Task<IEnumerable<Appointment>> GetTodayAppointmentsByDoctorIdAsync(
                string doctorMedicalId,
                DateTime date)
            {
                return await clinicDbContext.Appointments
                    .Where(a =>
                        a.DoctorMedicalId == doctorMedicalId &&
                        a.VisitDate.Date == date.Date)
                    .Include(a => a.Patient)
                    .OrderBy(a => a.VisitDate)
                    .ToListAsync();
            }

            public async Task<Appointment?> GetByIdAsync(Guid id)
            {
                return await clinicDbContext.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }

            public async Task UpdateAsync(Appointment appointment)
            {
                clinicDbContext.Appointments.Update(appointment);
                await clinicDbContext.SaveChangesAsync();
            }

            public async Task SaveChangesAsync()
            {
            await clinicDbContext.SaveChangesAsync();
            }
        }
    }
