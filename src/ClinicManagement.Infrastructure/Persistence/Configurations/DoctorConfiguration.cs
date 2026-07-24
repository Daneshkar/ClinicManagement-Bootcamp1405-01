using System;
using System.Collections.Generic;
using System.Text;
using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ClinicManagement.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>

{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");


        builder.HasKey(d => d.MedicalID);


        builder.Property(d => d.MedicalID)
            .IsRequired()
            .HasMaxLength(20)
            .IsUnicode(false);
            

   

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);



        builder.Property(d => d.Fee)
            .IsRequired()
            .HasColumnType("decimal(18,2)");



        builder.Property(d => d.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

    }


}
