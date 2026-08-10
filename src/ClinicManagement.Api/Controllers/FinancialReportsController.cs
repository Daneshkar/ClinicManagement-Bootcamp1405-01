using ClinicManagement.Application.DTOs.Reports;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Api.Controllers
{
    [ApiController]
    [Route("api/financial-reports")]
    public class FinancialReportsController : ControllerBase
    {
        private readonly IFinancialReportService _financialReportService;

        public FinancialReportsController(IFinancialReportService financialReportService)
        {
            _financialReportService = financialReportService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FinancialReportResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFinancialReport([FromQuery] GetFinancialReportRequest request)
        {
            var result = await _financialReportService.GetFinancialReportAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            // Assuming result.Error has a Code, Type, or Status identifier
            return result.Error?.Code switch
            {
                "NotFound" or "404" => NotFound(result.Error),
                _ => BadRequest(result.Error)
            };
        }
    }
}