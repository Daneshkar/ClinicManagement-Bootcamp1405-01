using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ClinicManagement.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        #region [- Fields -]
        private readonly ClinicDbContext _context; 
        #endregion

        #region [- PatientRepository -]
        public PatientRepository(ClinicDbContext context)
        {
            _context = context;
        } 
        #endregion

        #region [- AddAsync -]
        public async Task AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        } 
        #endregion

        #region [- DeleteAsync -]
        public async Task DeleteAsync(Patient patient)
        {
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region [- ExistsByNationalCodeAsync -]
        public async Task<bool> ExistsByNationalCodeAsync(string nationalCode)
        {
            return await _context.Patients.
            AnyAsync(p => p.NationalCode == nationalCode);
        } 
        #endregion

        #region [- GetAllAsync -]
        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
            .AsNoTracking()
            .ToListAsync();
        }
        #endregion

        #region [- GetByNationalCodeAsync -]
        public async Task<Patient?> GetByNationalCodeAsync(string nationalCode)
        {
            return await _context.Patients.
            FirstOrDefaultAsync(p => p.NationalCode == nationalCode);
        } 
        #endregion

        #region [- UpdateAsync -]
        public async Task UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
        } 
        #endregion
    }
}
