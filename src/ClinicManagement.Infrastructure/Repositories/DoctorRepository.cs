using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories;

public class DoctorRepository: IDoctorRepository

{
    private readonly ClinicDbContext _context;

    public DoctorRepository(ClinicDbContext context)
    {
        _context = context;
    }
    
    public async Task<Doctor?> GetByMedicalIdAsync(string medicallId)
    {
        return await _context.Doctors.
            FirstOrDefaultAsync(d => d.MedicalID == medicallId);
    }

    public async Task<bool> ExistsByMedicalIdAsync(string medicalId)
    {
        return await _context.Doctors.
            AnyAsync(d=> d.MedicalID == medicalId);
    }

    public async Task AddAsync(Doctor doctor)
    {
        await _context.Doctors.AddAsync(doctor);
        await _context.SaveChangesAsync();
    }
}