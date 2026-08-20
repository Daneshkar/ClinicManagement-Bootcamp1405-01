using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.HasOne(rt => rt.Doctor)
            .WithMany()
            .HasForeignKey(rt => rt.DoctorMedicalId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}