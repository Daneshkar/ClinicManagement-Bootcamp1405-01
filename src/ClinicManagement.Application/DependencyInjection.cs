using ClinicManagement.Application.Interfaces;
using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ClinicManagement.Application; // اینجا باید دقیقاً همان نامی باشد که در csproj تعریف شده

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //To Do: add services later
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IFinancialReportService, FinancialReportService>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}