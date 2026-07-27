using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Domain.Entities
{
    public class Patient
    {
        public required string NationalCode { get; set; }
        public required string Name { get; set; }
        public string? Phone { get; set; }
    }
}
