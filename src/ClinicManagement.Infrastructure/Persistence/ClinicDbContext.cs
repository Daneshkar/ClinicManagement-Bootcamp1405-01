using System;
using System.Collections.Generic;
using System.Text;
using ClinicManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace ClinicManagement.Infrastructure.Persistence;

public class ClinicDbContext : DbContext
{
   
     public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options)
    { 

    }

    public DbSet<Doctor> Doctors { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
           typeof(ClinicDbContext).Assembly);


        base.OnModelCreating(modelBuilder);
    }
}
