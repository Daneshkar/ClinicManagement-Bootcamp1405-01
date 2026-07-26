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
        return doctors.Select(d => new DoctorGetResponseDto(d.MedicalId, d.Name, d.Fee));
    }
    
    public async Task<DoctorGetResponseDto> GetDoctorByMedicalIdAsync(string medicalId)
    {
        if (String.IsNullOrEmpty(medicalId) || String.IsNullOrWhiteSpace(medicalId))
        {
            return null;
        }
        string trimmedMedicalId = medicalId.Trim();
        var doctor = await _doctorRepository.GetByMedicalIdAsync(trimmedMedicalId);
        if (doctor == null)
        {
            return null;
        }
            
        return  new DoctorGetResponseDto(doctor.MedicalId, doctor.Name, doctor.Fee);
    }
    


    public async Task<DoctorUpdateResponseDto> UpdateDoctorAsync(string medicalId, DoctorUpdateRequestDto request)
    {
        // checking input validness
        
        if (String.IsNullOrWhiteSpace(medicalId))
        {
            return new DoctorUpdateResponseDto(false, "Invalid Input");
        }

        if (String.IsNullOrWhiteSpace(request.Name))
        {
            return new DoctorUpdateResponseDto(false, "Invalid input for Name field");
        }

        if (request.Fee < 0)
        {
            return new DoctorUpdateResponseDto(false, "Fee cannot be negative");
        }
        
        // trimming input
        string trimmedName = request.Name.Trim();
        string trimmedMedicalId = medicalId.Trim();
        
        
        var doctor =  await _doctorRepository.GetByMedicalIdAsync(trimmedMedicalId);
        
        // checking for designated  availability
        if (doctor == null)
        {
            return new DoctorUpdateResponseDto(false, $"Doctor not found with medicalId:{medicalId}");
        }
        
        // updating the record fields with input
        doctor.Name = trimmedName;
        doctor.Fee = request.Fee;
        
        // call 
        await _doctorRepository.UpdateAsync(doctor);
        
        return new DoctorUpdateResponseDto(true, "Doctor updated successfully");
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

        await _doctorRepository.DeleteAsync(doctor);
        return  new DoctorDeleteResponseDto(true, "Doctor deleted successfully");


    }



}