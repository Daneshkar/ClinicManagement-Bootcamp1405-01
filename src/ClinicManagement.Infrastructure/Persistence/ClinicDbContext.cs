using System;
using System.Collections.Generic;
using System.Text;
using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ClinicManagement.Infrastructure.Persistence;

public class ClinicDbContext : DbContext
{
   
     public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
    { 

    }

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Secretary> Secretaries { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
           typeof(ClinicDbContext).Assembly);


        base.OnModelCreating(modelBuilder);
    }
}
