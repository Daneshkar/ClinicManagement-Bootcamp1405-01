using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Interfaces.Repository
{
    public interface IPatientRepository
    {
        public Task<Patient?> GetByNationalCodeAsync(string nationalCode);
        public Task<bool> ExistsByNationalCodeAsync(string nationalCode);
        public Task<IEnumerable<Patient>> GetAllAsync();
        public Task AddAsync(Patient patient);
        public Task UpdateAsync(Patient patient);
        public Task DeleteAsync(Patient patient);
    }
}
