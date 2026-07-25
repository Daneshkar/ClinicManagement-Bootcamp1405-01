using ClinicManagement.Application.DTOs.Doctors;
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


        public Task<IEnumerable<Doctor>> GetAllAsync();
         public Task<Doctor?> UpdateAsync(Doctor doctor);

        public Task<DoctorDeleteResponseDto> DeleteAsync(Doctor doctor);

    }
}
