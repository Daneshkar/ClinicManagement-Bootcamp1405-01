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

            return new PatientCreateResponseDto(true, "Patient created successfully");
        }

        public async Task<IEnumerable<PatientGetResponseDto>> GetAllPatientsAsync()
        {
            var patients = await _patientRepository.GetAllAsync();

            if (!patients.Any())
            {
                return Enumerable.Empty<PatientGetResponseDto>();
            }

            return patients.Select(p => new PatientGetResponseDto(
                p.NationalCode,
                p.Name,
                p.Phone
            ));
        }

        public async Task<PatientGetResponseDto?> GetPatientByNationalCodeAsync(
    string nationalCode)
        {
            string trimmedNationalCode = nationalCode.Trim();

            var patient = await _patientRepository
                .GetByNationalCodeAsync(trimmedNationalCode);

            if (patient == null)
            {
                return null;
            }

            return new PatientGetResponseDto(
                patient.NationalCode,
                patient.Name,
                patient.Phone
            );
        }

        public async Task<PatientUpdateResponseDto> UpdatePatientAsync(
    string nationalCode,
    PatientUpdateRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(nationalCode))
            {
                return new PatientUpdateResponseDto(false, "Invalid national code.");
            }

            string trimmedNationalCode = nationalCode.Trim();
            string trimmedName = request.Name.Trim();
            string trimmedPhone = request.Phone.Trim();

            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                return new PatientUpdateResponseDto(false, "Name is required.");
            }

            var patient = await _patientRepository
                .GetByNationalCodeAsync(trimmedNationalCode);

            if (patient == null)
            {
                return new PatientUpdateResponseDto(false, "Patient not found.");
            }

            patient.Name = trimmedName;
            patient.Phone = trimmedPhone;

            await _patientRepository.UpdateAsync(patient);

            return new PatientUpdateResponseDto(true, "Patient Updated successfully!");
        }

        public async Task<PatientDeleteResponseDto> DeletePatientAsync(string nationalCode)
        {
            if (string.IsNullOrWhiteSpace(nationalCode))
            {
                return new PatientDeleteResponseDto(false, "Invalid national code.");
            }

            string trimmedNationalCode = nationalCode.Trim();

            var patient = await _patientRepository
                .GetByNationalCodeAsync(trimmedNationalCode);

            if (patient == null)
            {
                return new PatientDeleteResponseDto(false, "Patient not found.");
            }

            await _patientRepository.DeleteAsync(patient);

            return new PatientDeleteResponseDto(true, "Patient deleted successfully!");
        }




    }
}
