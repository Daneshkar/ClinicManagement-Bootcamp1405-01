namespace ClinicManagement.Application.Interfaces.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool IsMatch(string password, string passwordHash);
}