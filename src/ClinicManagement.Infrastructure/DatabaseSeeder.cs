using ClinicManagement.Application.Interfaces.Services;
using ClinicManagement.Infrastructure.Persistence;
using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicManagement.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedInitialSecretaryAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        if (!await context.Secretaries.AnyAsync())
        {
            var defaultSecretary = Secretary.Create(
                userName: "admin_secretary",
                passwordHash: passwordHasher.HashPassword("Secretary123!"),
                name: "Default Secretary"
            );

            await context.Secretaries.AddAsync(defaultSecretary);
            await context.SaveChangesAsync();
        }
    }
}