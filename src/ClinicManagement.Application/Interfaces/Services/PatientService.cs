using ClinicManagement.Application.DTOs.Patients;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Interfaces.Services
{
     public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }


        public async Task<PatientCreateResponseDto> CreatePatientAsync(
    PatientCreateRequestDto request)
        {
            string nationalCode = request.NationalCode.Trim();
            string name = request.Name.Trim();
            string phone = request.Phone.Trim();

            if (string.IsNullOrEmpty(nationalCode) || string.IsNullOrEmpty(name))
            {
                return new PatientCreateResponseDto(
                    false,
                    "National Code and Name are required.");
            }

            bool exists = await _patientRepository
                .ExistsByNationalCodeAsync(nationalCode);

            if (exists)
            {
                return new PatientCreateResponseDto(
                    false,
                    "Patient with this National Code already exists.");
            }

            var patient = new Patient
            {
                NationalCode = nationalCode,
                Name = name,
                Phone = phone
            };

            await _patientRepository.AddAsync(patient);

            return new PatientCreateResponseDto(true, null);
        }

    }
}
