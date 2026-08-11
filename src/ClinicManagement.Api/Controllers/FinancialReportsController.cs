using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Reports;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Api.Controllers;

[ApiController]
[Route("api/financial-reports")]
public class FinancialReportsController : ControllerBase
{
    private readonly IFinancialReportService _financialReportService;

    public FinancialReportsController(IFinancialReportService financialReportService)
    {
        _financialReportService = financialReportService;
    }

    /// <summary>
    /// Generates a financial report for the specified doctor(s) over a given time period.
    /// </summary>
    /// <param name="request">Query parameters containing doctor IDs, time period, and optional custom date bounds.</param>
    /// <returns>Aggregated financial report including revenue per doctor and grand total.</returns>
    /// <response code="200">Returns the generated financial report.</response>
    /// <response code="400">If validation fails or request parameters are invalid.</response>
    /// <response code="404">If any provided doctor medical ID is not found in the system.</response>
    [HttpGet]
    [ProducesResponseType(typeof(FinancialReportResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFinancialReport([FromQuery] GetFinancialReportRequest request)
    {
        var result = await _financialReportService.GetFinancialReportAsync(request);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        // Map domain/validation error types to appropriate HTTP status responses
        return result.Error.Type switch
        {
            ErrorType.NotFound => NotFound(new { result.Error.Code, result.Error.Message }),
            ErrorType.Validation => BadRequest(new { result.Error.Code, result.Error.Message }),
            ErrorType.Conflict => Conflict(new { result.Error.Code, result.Error.Message }),
            ErrorType.Failure => StatusCode(StatusCodes.Status500InternalServerError, new { result.Error.Code, result.Error.Message }),
            _ => BadRequest(new { result.Error.Code, result.Error.Message })
        };
    }
}