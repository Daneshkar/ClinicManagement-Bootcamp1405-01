using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Doctors;
using ClinicManagement.Application.DTOs.Patients;
using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
namespace ClinicManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PatientGetResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _patientService.GetAllPatientsAsync();

            return Ok(result);
        }

        [HttpGet("{nationalCode}")]
        [ProducesResponseType(typeof(PatientGetResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] string nationalCode)
        {
            if (string.IsNullOrWhiteSpace(nationalCode))
            {
                return BadRequest("NationalCode is required.");
            }

            var patient = await _patientService.GetPatientByNationalCodeAsync(nationalCode);

            if (patient is null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PatientCreateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PatientCreateResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PatientCreateResponseDto), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] PatientCreateRequestDto request)
        {
            var result = await _patientService.CreatePatientAsync(request);

            if (!result.IsSuccess)
            {
                // Duplicate NationalCode -> 409 Conflict
                if (result.Message is not null &&
                    result.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                {
                    return Conflict(result);
                }

                // Missing/empty required fields -> 400 Bad Request
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("{nationalCode}")]
        [ProducesResponseType(typeof(PatientUpdateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PatientUpdateResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PatientUpdateResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put([FromRoute] string nationalCode, [FromBody] PatientUpdateRequestDto request)
        {
            var result = await _patientService.UpdatePatientAsync(nationalCode, request);

            if (!result.IsSuccess)
            {
                // Patient not found -> 404 Not Found
                if (result.Message is not null &&
                    result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(result);
                }

                // Invalid input (e.g., empty Name) -> 400 Bad Request
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{nationalCode}")]
        [ProducesResponseType(typeof(PatientDeleteResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PatientDeleteResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PatientDeleteResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] string nationalCode)
        {
            var result = await _patientService.DeletePatientAsync(nationalCode);

            if (!result.IsSuccess)
            {
                // Empty/whitespace nationalCode -> 400 Bad Request
                if (result.Message is not null &&
                    result.Message.Contains("required", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(result);
                }

                // Patient not found -> 404 Not Found
                if (result.Message is not null &&
                    result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(result);
                }

                // Fallback: unspecified failure -> 400 Bad Request
                return BadRequest(result);
            }

            return Ok(result);
        }
        
        // --- Custom Error Handling Method ---
        // This method is assumed to exist within the controller or a base controller.
        // It maps your custom Error object to appropriate HTTP responses.
        private IActionResult HandleError(Error error)
        {
            // Assuming ErrorType is an enum or similar structure
            // and that Error has properties like 'Code', 'Message', and 'Type'.
            // You might need to adjust this based on your exact Error and ErrorType structure.
            return error.Type switch
            {
                ErrorType.NotFound => StatusCode(StatusCodes.Status404NotFound, error),
                ErrorType.Conflict => StatusCode(StatusCodes.Status409Conflict, error),
                ErrorType.Validation => StatusCode(StatusCodes.Status400BadRequest,
                    error),
                _ => StatusCode(StatusCodes.Status400BadRequest, error) // Default for other unexpected errors
            };
        }
        
    }
}
