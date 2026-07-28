using ClinicManagement.Application.DTOs.Doctors;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Interfaces.Repository
{
    public interface IDoctorRepository
    {
        Task<Doctor?> GetByMedicalIdAsync(string medicalId);
        Task<bool> ExistsByMedicalIdAsync(string medicalId);
        Task AddAsync(Doctor doctor);
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(Doctor doctor);

    }
}
