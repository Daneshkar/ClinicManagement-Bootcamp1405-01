using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Application; // اینجا باید دقیقاً همان نامی باشد که در csproj تعریف شده

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //To Do: add services later
        services.AddScoped<IDoctorService, DoctorService>();
        return services;
    }
}