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
    
    public async Task<Doctor?> GetByMedicalIdAsync(string medicalId)
    {
        return await _context.Doctors.
            FirstOrDefaultAsync(d => d.MedicalID == medicalId);
    }

    public async Task<bool> ExistsByMedicalIdAsync(string medicalId)
    {
        return await _context.Doctors.
            AnyAsync(d=> d.MedicalId == medicalId);
    }

    public async Task AddAsync(Doctor doctor)
    {
        await _context.Doctors.AddAsync(doctor);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Doctor>> GetAllAsync()
    {
        return await _context.Doctors
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task UpdateAsync(Doctor doctor)
    {
        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Doctor doctor)
    {
        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
    }



}