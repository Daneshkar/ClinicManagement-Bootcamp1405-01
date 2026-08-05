using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Patients;
using ClinicManagement.Application.Interfaces.Repository;
using FluentValidation;

namespace ClinicManagement.Application.Interfaces.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        private readonly IPasswordHasher _passwordHasher;

        private readonly IValidator<PatientSignupRequest> _patientSignupRequestValidator;
        private readonly IValidator<GetPatientByNationalCodeRequest> _getPatientByNationalCodeRequestValidator;
        private readonly IValidator<UpdatePatientRequest> _updatePatientRequestValidator;
        private readonly IValidator<DeletePatientRequest> _deletePatientRequestValidator;


        public PatientService(
            IPatientRepository patientRepository,
            IPasswordHasher passwordHasher,
            IValidator<PatientSignupRequest> patientSignupRequestValidator,
            IValidator<GetPatientByNationalCodeRequest> getPatientByNationalCodeRequestValidator,
            IValidator<UpdatePatientRequest> updatePatientRequestValidator,
            IValidator<DeletePatientRequest> deletePatientRequestValidator)
        {
            _patientRepository = patientRepository;
            _passwordHasher = passwordHasher;

            _patientSignupRequestValidator = patientSignupRequestValidator;
            _getPatientByNationalCodeRequestValidator = getPatientByNationalCodeRequestValidator;
            _updatePatientRequestValidator = updatePatientRequestValidator;
            _deletePatientRequestValidator = deletePatientRequestValidator;
        }


        public async Task<Result<PatientResponse>> SignupAsync(
            PatientSignupRequest request)
        {
            throw new NotImplementedException();
        }


        public async Task<Result<IEnumerable<PatientResponse>>> GetAllPatientsAsync(
            GetAllPatientsRequest request)
        {
            throw new NotImplementedException();
        }


        public async Task<Result<PatientResponse>> GetPatientByNationalCodeAsync(
            GetPatientByNationalCodeRequest request)
        {
            throw new NotImplementedException();
        }


        public async Task<Result<PatientResponse>> UpdatePatientAsync(
            UpdatePatientRequest request)
        {
            throw new NotImplementedException();
        }


        public async Task<Result<PatientResponse>> DeletePatientAsync(
            DeletePatientRequest request)
        {
            throw new NotImplementedException();
        }
    }
}