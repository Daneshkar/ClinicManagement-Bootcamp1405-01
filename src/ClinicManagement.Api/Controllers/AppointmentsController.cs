using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Appointments;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>
    /// Retrieves available 1-hour appointment slots for a doctor over the next 7 days.
    /// </summary>
    /// <param name="request">The route parameter containing doctor's medical ID.</param>
    /// <returns>A list of open appointment slots for the doctor.</returns>
    [HttpGet("available-slots/{MedicalId}")]
    [ProducesResponseType(typeof(DoctorAvailableSlotsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAvailableSlots([FromRoute] GetDoctorAvailableSlotsRequest request)
    {
        var result = await _appointmentService.GetAvailableSlotsAsync(request);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return HandleError(result.Error);
    }

    /// <summary>
    /// Books a 1-hour appointment slot for a patient.
    /// </summary>
    /// <param name="request">The booking request details including doctor, patient, and date/time.</param>
    /// <returns>Confirmation payload detailing success or validation failures.</returns>
    [HttpPost("book")]
    [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> BookAppointment([FromBody] AppointmentCreateRequest request)
    {
        var result = await _appointmentService.BookAppointmentAsync(request);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return HandleError(result.Error);
    }

    // --- Custom Error Handling Method ---
    private IActionResult HandleError(Error error)
    {
        return error.Type switch
        {
            ErrorType.NotFound => StatusCode(StatusCodes.Status404NotFound, error),
            ErrorType.Conflict => StatusCode(StatusCodes.Status409Conflict, error),
            ErrorType.Validation => StatusCode(StatusCodes.Status400BadRequest, error),
            _ => StatusCode(StatusCodes.Status400BadRequest, error)
        };
    }
}