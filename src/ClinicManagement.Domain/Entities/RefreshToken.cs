namespace ClinicManagement.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string UserIdentifier { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public bool IsUsed { get; private set; }

  

    // Parameterless constructor required by EF Core to prevent Guid.NewGuid() on DB reads
    private RefreshToken() { }

    public static RefreshToken Create(string userIdentifier, string token, TimeSpan lifetime)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserIdentifier = userIdentifier,
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