using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Domain.Entities
{
    public class Doctor

    {
        public required string Name { get; set; }
        public required string MedicalID { get; set; }
        public string PasswordHash { get; set; }
        public decimal Fee { get; set; }
    }


}
