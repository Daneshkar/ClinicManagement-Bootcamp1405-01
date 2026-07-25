using ClinicManagement.Domain.Entities;
using ClinicManagement.Application.DTOs.Doctors;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;

namespace ClinicManagement.Application.Services;

public class DoctorService : IDoctorService
{
    private IDoctorRepository _doctorRepository;
    private IPasswordHasher _passwordHasher;

    public DoctorService(IDoctorRepository doctorRepository, IPasswordHasher passwordHasher)
    {
        _doctorRepository = doctorRepository;
        _passwordHasher = passwordHasher;
    }


    /// <summary>
    /// Registers a new doctor user in the system after validating uniqueness and hashing the password.
    /// </summary>
    /// <param name="request">The doctor registration details.</param>
    /// <returns>A response containing the outcome and registration status.</returns>
    public async Task<DoctorSignupResponseDto> SignupAsync(DoctorSignupRequestDto request)
    {
        bool exists = await _doctorRepository.ExistsByMedicalIdAsync(request.MedicalID);
        if (exists)
        {
            return new DoctorSignupResponseDto()
            {
                MedicalID = request.MedicalID,
                Name = request.Name,
                Fee = request.Fee,
                IsSuccess = false,
                Message = $"duplicate medical id: {request.MedicalID}"
            };
        }

        string passwordHash = _passwordHasher.HashPassword(request.Password);

        var doctor = new Doctor()
        {
            MedicalId = request.MedicalID,
            Name = request.Name,
            Fee = request.Fee,
            PasswordHash = passwordHash,
        };
        await _doctorRepository.AddAsync(doctor);

        return new DoctorSignupResponseDto()
        {
            MedicalID = request.MedicalID,
            Name = request.Name,
            Fee = request.Fee,
            IsSuccess = true,
            Message = "doctor user registered successfully!"
        };


    }
    public async Task<IEnumerable<DoctorGetResponseDto>> GetAllDoctorsAsync()
    {

        var doctors = await _doctorRepository.GetAllAsync();

        if (!doctors.Any())
        {
            return Enumerable.Empty<DoctorGetResponseDto>();

        }
        var result = doctors.Select(doctor => new DoctorGetResponseDto(doctor.MedicalId, doctor.Name, doctor.Fee));
        return result;

    }
    public async Task<DoctorGetResponseDto> GetDoctorByMedicalIdAsync(string medicalId)
    {

        if (string.IsNullOrWhiteSpace(medicalId))
        {

            return null;
        }
         medicalId = medicalId.Trim();
        var doctor = await _doctorRepository.GetByMedicalIdAsync(medicalId);
        if (doctor == null)
        {

            return null;

        }

        if (doctor == null)
        {
            return null;
        }

        return new DoctorGetResponseDto(
       doctor.MedicalId,
       doctor.Name,
       doctor.Fee
   );

    }



    public async Task<DoctorUpdateResponseDto> UpdateDoctorAsync(string medicalId, DoctorUpdateRequestDto request)
    {

        if (string.IsNullOrWhiteSpace(request.Name))
        {

            DoctorUpdateResponseDto updateResponseDto = new DoctorUpdateResponseDto(false, "Invalid doctor name.");


            return updateResponseDto;
        }

        if (request.Fee < 0)
        {
            DoctorUpdateResponseDto updateResponseDto = new DoctorUpdateResponseDto(false, "Fee cannot be negative.");

            return updateResponseDto;
        }

        var doctor = await _doctorRepository.GetByMedicalIdAsync(medicalId);
        if (doctor == null)
        {
            return new DoctorUpdateResponseDto(false, "Doctor not found");
        }

        if (doctor.Name == request.Name && doctor.Fee == request.Fee)
        {

            return new DoctorUpdateResponseDto(true, "No changes were made.");
        }
        if (request == null)
        {
            return new DoctorUpdateResponseDto(false, "Invalid request.");
        }

        doctor.Name = request.Name;
        doctor.Fee = request.Fee;
        await _doctorRepository.UpdateAsync(doctor);

        return new DoctorUpdateResponseDto(true, "Doctor updated successfully.");
    }
    public async Task<DoctorDeleteResponseDto> DeleteDoctorAsync(string medicalId)

    {
        if (string.IsNullOrWhiteSpace(medicalId))
        {
            return new DoctorDeleteResponseDto(false, "Invalid medical id.");
        }

        var doctor = await _doctorRepository.GetByMedicalIdAsync(medicalId);

        if (doctor == null)
        {
            return new DoctorDeleteResponseDto(false, "Doctor not found.");
        }

        return await _doctorRepository.DeleteAsync(doctor);


    }



}