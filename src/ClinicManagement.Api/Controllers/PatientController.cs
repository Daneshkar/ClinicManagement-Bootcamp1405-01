using ClinicManagement.Application.DTOs.Patients;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
namespace ClinicManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
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
    }
}
