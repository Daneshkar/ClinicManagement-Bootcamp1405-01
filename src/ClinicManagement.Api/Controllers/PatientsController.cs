using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Patients;
using ClinicManagement.Application.Interfaces.Services;
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
        [ProducesResponseType(typeof(IEnumerable<PatientResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery]GetAllPatientsRequest request)
        {
            var result = await _patientService.GetAllPatientsAsync(request);

            if (!result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return HandleError(result.Error);
        }
        [HttpGet("{NationalCode}")]
        [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult>  GetByNationalCode([FromRoute] string nationalCode)
        {
            var request = new GetPatientByNationalCodeRequest(nationalCode);
            var result = await _patientService.GetPatientByNationalCodeAsync(request);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return HandleError(result.Error);


        }

        [HttpPost("signup")]
        [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Signup([FromBody] PatientSignupRequest request)
        {
            var result = await _patientService.SignupAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            return HandleError(result.Error);


        }

        [HttpPut("update")]
        [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdatePatientRequest  request)
        {
            var result = await _patientService.UpdatePatientAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result.Value);

            }
            return HandleError(result.Error);

        }

        [HttpDelete("{NationalCode}")]
        [ProducesResponseType(typeof(PatientResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] DeletePatientRequest request)
        {
            var result = await _patientService.DeletePatientAsync(request);

            if (result.IsSuccess)

            {
                return Ok(result.Value);

            }
            return HandleError(result.Error);                

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
