using ClinicManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.DTOs.Reports
{
    public record GetFinancialReportRequest(
    List<string> DoctorMedicalIds,
    TimePeriodOption Period = TimePeriodOption.AllTime,
    DateTime? CustomFromDate = null,
    DateTime? CustomToDate = null
);

    public record DoctorFinancialReportDto(
        string DoctorMedicalId,
        string DoctorName,
        decimal Fee,
        int VisitedAppointmentCount,
        decimal Revenue
    );

    public record FinancialReportResponse(
        TimePeriodOption Period,
        DateTime? FromDate,
        DateTime? ToDate,
        List<DoctorFinancialReportDto> DoctorReports, decimal GrandTotal
);

}
