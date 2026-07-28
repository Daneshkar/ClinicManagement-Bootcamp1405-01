using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Interfaces.Repository
{
    public interface IPatientRepository
    {
        Task<Patient?> GetByNationalCodeAsync(string nationalCode);
        Task<bool> ExistsByNationalCodeAsync(string nationalCode);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task AddAsync(Patient patient);
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(Patient patient);
    }
}
