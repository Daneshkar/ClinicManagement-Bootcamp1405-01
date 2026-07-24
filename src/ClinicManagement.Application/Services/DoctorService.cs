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
            MedicalID = request.MedicalID,
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
}