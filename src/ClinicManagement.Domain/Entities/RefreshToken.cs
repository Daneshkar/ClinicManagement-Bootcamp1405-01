namespace ClinicManagement.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string DoctorMedicalId { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public bool IsUsed { get; private set; }

    // Navigation Property
    public Doctor? Doctor { get; private set; }

    // Parameterless constructor required by EF Core to prevent Guid.NewGuid() on DB reads
    private RefreshToken() { }

    public static RefreshToken Create(string doctorMedicalId, string token, TimeSpan lifetime)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            DoctorMedicalId = doctorMedicalId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.Add(lifetime),
            IsRevoked = false,
            IsUsed = false
        };
    }

    // Domain Methods for Encapsulation
    public void MarkAsUsed() => IsUsed = true;
    public void Revoke() => IsRevoked = true;
}