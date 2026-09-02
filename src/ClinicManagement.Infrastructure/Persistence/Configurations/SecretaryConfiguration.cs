using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicManagement.Infrastructure.Persistence.Configurations
{
    public class SecretaryConfiguration:IEntityTypeConfiguration<Secretary>
    {
        public void Configure(EntityTypeBuilder<Secretary>builder)
        {
            builder.ToTable("Secretaries");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.UserName)
    .IsRequired()
    .HasMaxLength(50);
            builder.HasIndex(s => s.UserName)
    .IsUnique();
            builder.Property(s => s.Name)
    .IsRequired()
    .HasMaxLength(100);
            builder.Property(s => s.PasswordHash)
    .IsRequired()
    .HasMaxLength(255);

        }

    }
}
