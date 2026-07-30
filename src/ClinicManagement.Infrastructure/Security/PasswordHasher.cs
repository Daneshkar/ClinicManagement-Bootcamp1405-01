using ClinicManagement.Application.Interfaces.Services;
namespace ClinicManagement.Infrastructure.Security;

public class PasswordHasher: IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool IsMatch(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}