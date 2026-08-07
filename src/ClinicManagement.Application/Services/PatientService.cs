using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Patients;
using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IValidator<PatientSignupRequest> _patientSignupRequestValidator;
    private readonly IValidator<GetPatientByNationalCodeRequest> _getPatientByNationalCodeRequestValidator;
    private readonly IValidator<UpdatePatientRequest> _updatePatientRequestValidator;
    private readonly IValidator<DeletePatientRequest> _deletePatientRequestValidator;

    public PatientService(
        IPatientRepository patientRepository,
        IValidator<PatientSignupRequest> patientSignupRequestValidator,
        IValidator<GetPatientByNationalCodeRequest> getPatientByNationalCodeRequestValidator,
        IValidator<UpdatePatientRequest> updatePatientRequestValidator,
        IValidator<DeletePatientRequest> deletePatientRequestValidator)
    {
        _patientRepository = patientRepository;
        _patientSignupRequestValidator = patientSignupRequestValidator;
        _getPatientByNationalCodeRequestValidator = getPatientByNationalCodeRequestValidator;
        _updatePatientRequestValidator = updatePatientRequestValidator;
        _deletePatientRequestValidator = deletePatientRequestValidator;
    }

    public async Task<Result<PatientResponse>> SignupAsync(PatientSignupRequest request)
    {
        var validationResult = await _patientSignupRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return FormatValidationErrors(validationResult.Errors);
        }

        bool exists = await _patientRepository.ExistsByNationalCodeAsync(request.NationalCode);
        if (exists)
        {
            return Error.Conflict(
                "Patient.DuplicateNationalCode",
                $"A patient with National Code '{request.NationalCode}' already exists.");
        }

        var patient = new Patient
        {
            NationalCode = request.NationalCode,
            Name = request.Name,
            Phone = request.Phone
        };

        await _patientRepository.AddAsync(patient);

        return Result<PatientResponse>.Success(
            new PatientResponse(patient.NationalCode, patient.Name, patient.Phone));
    }

    public async Task<Result<IEnumerable<PatientResponse>>> GetAllPatientsAsync(GetAllPatientsRequest request)
    {
        var patients = await _patientRepository.GetAllAsync();

        var responseList = patients.Select(p => new PatientResponse(
            p.NationalCode,
            p.Name,
            p.Phone));

        return Result<IEnumerable<PatientResponse>>.Success(responseList);
    }

    public async Task<Result<PatientResponse>> GetPatientByNationalCodeAsync(GetPatientByNationalCodeRequest request)
    {
        var validationResult = await _getPatientByNationalCodeRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return FormatValidationErrors(validationResult.Errors);
        }

        var patient = await _patientRepository.GetByNationalCodeAsync(request.NationalCode);
        if (patient is null)
        {
            return Error.NotFound(
                "Patient.NotFound",
                $"Patient with National Code '{request.NationalCode}' was not found.");
        }

        return Result<PatientResponse>.Success(
            new PatientResponse(patient.NationalCode, patient.Name, patient.Phone));
    }

    public async Task<Result<PatientResponse>> UpdatePatientAsync(UpdatePatientRequest request)
    {
        var validationResult = await _updatePatientRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return FormatValidationErrors(validationResult.Errors);
        }

        var patient = await _patientRepository.GetByNationalCodeAsync(request.NationalCode);
        if (patient is null)
        {
            return Error.NotFound(
                "Patient.NotFound",
                $"Patient with National Code '{request.NationalCode}' was not found.");
        }

        patient.Name = request.Name;
        patient.Phone = request.Phone;

        await _patientRepository.UpdateAsync(patient);

        return Result<PatientResponse>.Success(
            new PatientResponse(patient.NationalCode, patient.Name, patient.Phone));
    }

    public async Task<Result<PatientResponse>> DeletePatientAsync(DeletePatientRequest request)
    {
        var validationResult = await _deletePatientRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return FormatValidationErrors(validationResult.Errors);
        }

        var patient = await _patientRepository.GetByNationalCodeAsync(request.NationalCode);
        if (patient is null)
        {
            return Error.NotFound(
                "Patient.NotFound",
                $"Patient with National Code '{request.NationalCode}' was not found.");
        }

        var response = new PatientResponse(patient.NationalCode, patient.Name, patient.Phone);

        await _patientRepository.DeleteAsync(patient);

        return Result<PatientResponse>.Success(response);
    }

    private static Error FormatValidationErrors(List<ValidationFailure> failures)
    {
        string aggregatedErrors = string.Join(" | ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
        return Error.Validation(
            "Model.Validation",
            $"Input validation failed: {aggregatedErrors}");
    }
}