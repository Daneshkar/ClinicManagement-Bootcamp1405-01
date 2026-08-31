using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Treatment;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.Api.Controllers
{
    [ApiController]
    [Route("api/treatment")]
    [Produces("application/json")]
    [Authorize]
    public class TreatmentController : ControllerBase
    {
        private readonly ITreatmentService _treatmentService;

        public TreatmentController(ITreatmentService treatmentService)
        {
            _treatmentService = treatmentService;
        }

        /// <summary>
        /// Retrieves the authenticated doctor's appointment queue for the current day.
        /// </summary>
        [HttpGet("today-appointments")]
        [ProducesResponseType(typeof(IEnumerable<TodayAppointmentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTodayAppointments()
        {
            var doctorMedicalId = GetDoctorMedicalId();

            var result = await _treatmentService.GetTodayAppointmentsAsync(doctorMedicalId, DateTime.UtcNow);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return HandleError(result.Error);
        }

        /// <summary>
        /// Records a prescription for a patient appointment belonging to the authenticated doctor.
        /// </summary>
        [HttpPost("prescription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterPrescription([FromBody] RegisterPrescriptionRequest request)
        {
            var doctorMedicalId = GetDoctorMedicalId();

            var result = await _treatmentService.RegisterPrescriptionAsync(doctorMedicalId, request, DateTime.UtcNow);

            if (result.IsSuccess)
            {
                return Ok(new { message = "Prescription registered successfully." });
            }

            return HandleError(result.Error);
        }

        /// <summary>
        /// Extracts the doctor's medical identifier from the authenticated user's claims.
        /// </summary>
        private string GetDoctorMedicalId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? string.Empty;
        }

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
                ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, error),
                ErrorType.Unauthorized => StatusCode(StatusCodes.Status401Unauthorized, error),
                _ => StatusCode(StatusCodes.Status400BadRequest, error) // Default for other unexpected errors
            };
        }
    }
}