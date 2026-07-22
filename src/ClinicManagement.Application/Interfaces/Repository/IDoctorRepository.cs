using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Interfaces.Repository
{
    public interface IDoctorRepository
    {

        public Task<Doctor?> GetByMedicalIdAsync(string medicallId);

        public Task<bool> ExistsByMedicalIdAsync(string medicalId);

        public Task AddAsync(Doctor doctor);

    }
}
