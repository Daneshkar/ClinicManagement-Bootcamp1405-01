using ClinicManagement.Application.DTOs.Appointments;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>
    /// Retrieves available 1-hour appointment slots for a doctor over the next 7 days.
    /// </summary>
    /// <param name="doctorMedicalId">The medical license/ID of the target doctor.</param>
    /// <returns>A list of open appointment slots for the doctor.</returns>
    [HttpGet("available-slots/{doctorMedicalId}")]
    [ProducesResponseType(typeof(DoctorAvailableSlotsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailableSlots(string doctorMedicalId)
    {
        var result = await _appointmentService.GetAvailableSlotsAsync(doctorMedicalId, DateTime.Today);
        
        if (result == null)
        {
            return NotFound(new { Message = $"No doctor was found with Medical ID '{doctorMedicalId}'." });
        }

        return Ok(result);
    }

    /// <summary>
    /// Books a 1-hour appointment slot for a patient.
    /// </summary>
    /// <param name="request">The booking request details including doctor, patient, and date/time.</param>
    /// <returns>Confirmation payload detailing success or validation failures.</returns>
    [HttpPost("book")]
    [ProducesResponseType(typeof(AppointmentCreateResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AppointmentCreateResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BookAppointment([FromBody] AppointmentCreateRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _appointmentService.BookAppointmentAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}