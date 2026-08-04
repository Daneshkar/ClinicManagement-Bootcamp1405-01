using Azure.Core;
using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Doctors;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpPost("signup")]
        [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Signup([FromBody] DoctorSignupRequest request)

        {
            var result = await _doctorService.SignupAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return HandleError(result.Error);
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DoctorResponse>), StatusCodes.Status200OK)]
       
        public async Task<IActionResult> GetAll([FromQuery] GetAllDoctorsRequest request)
        {
            var result = await _doctorService.GetAllDoctorsAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return HandleError(result.Error);
        }

        [HttpGet("{MedicalId}")]
        [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
           public async Task<IActionResult> GetByMedicalId(
                [FromRoute] string MedicalId)
        {
            var request = new GetDoctorByIdRequest(MedicalId);

            var result = await _doctorService.GetDoctorByMedicalIdAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return HandleError(result.Error);
        }
        

        [HttpPut]
        [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status404NotFound)]

        public async Task<IActionResult> Update(
           [FromBody] UpdateDoctorRequest request)
        {
            var result = await _doctorService.UpdateDoctorAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return HandleError(result.Error);
        }


        [HttpDelete("{MedicalId}")]
        [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DoctorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            [FromRoute] DeleteDoctorRequest request)
        {
         
             var result = await _doctorService.DeleteDoctorAsync(request);

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

