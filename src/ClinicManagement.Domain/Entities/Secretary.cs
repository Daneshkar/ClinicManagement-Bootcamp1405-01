using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Domain.Entities
{
    public class Secretary
    {
        public Guid Id {  get;private set; }
        public string UserName {  get; private set; }
        public string PasswordHash {  get; private set; }
        public string Name { get; private set; }

    }
}
