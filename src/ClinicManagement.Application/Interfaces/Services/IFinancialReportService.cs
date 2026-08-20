using ClinicManagement.Application.Common;
using ClinicManagement.Application.DTOs.Reports;

namespace ClinicManagement.Application.Interfaces.Services
{
    public interface IFinancialReportService
    {
        Task<Result<FinancialReportResponse>> GetFinancialReportAsync(GetFinancialReportRequest request);
    }
}
