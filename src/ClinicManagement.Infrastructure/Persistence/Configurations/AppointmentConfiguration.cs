using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        // Primary Key
        builder.HasKey(a => a.Id);

        // Composite Unique Index to prevent double-booking
        builder.HasIndex(a => new { a.DoctorMedicalId, a.VisitDate })
            .IsUnique();

        builder.Property(a => a.DoctorMedicalId)
            .IsRequired();

        builder.Property(a => a.PatientNationalCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.VisitDate)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Prescription)
            .IsRequired(false)
            .HasMaxLength(1000);

        // Unidirectional 1-to-Many Relationships
        builder.HasOne(a => a.Doctor)
            .WithMany()
            .HasForeignKey(a => a.DoctorMedicalId);

        builder.HasOne(a => a.Patient)
            .WithMany()
            .HasForeignKey(a => a.PatientNationalCode);
                
    }
}