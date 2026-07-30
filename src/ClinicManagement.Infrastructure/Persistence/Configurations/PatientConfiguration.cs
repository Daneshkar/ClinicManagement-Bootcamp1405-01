using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Infrastructure.Persistence.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        #region [- Configure -]
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(p => p.NationalCode);

            builder.Property(p => p.NationalCode)
            .IsRequired()
            .HasMaxLength(20)
            .IsUnicode(false);

            builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(p => p.Phone)
                .IsRequired(false)
                .HasMaxLength(15);
        } 
        #endregion
    }
}
