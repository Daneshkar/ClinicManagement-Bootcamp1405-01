namespace ClinicManagement.Application.Interfaces.Services;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyHash(string password, string passwordHash);
}