using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.UserIdentifier)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(rt => rt.UserIdentifier);
        builder.Property(rt => rt.Token)
            . IsRequired()
            . HasMaxLength (256);
        builder.HasIndex(rt => rt.Token)
            . IsUnique();
    }

}