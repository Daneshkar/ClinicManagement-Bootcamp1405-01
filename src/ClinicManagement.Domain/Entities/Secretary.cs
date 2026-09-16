namespace ClinicManagement.Domain.Entities;

public class Secretary
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; }
    public string PasswordHash { get; private set; }
    public string Name { get; private set; }

    // Required by EF Core
    private Secretary() { }

    public static Secretary Create(string userName, string passwordHash, string name)
    {
        return new Secretary()
        {
            Id = Guid.NewGuid(),
            UserName=userName,
            PasswordHash=passwordHash,
            Name=name
        };

    }
}