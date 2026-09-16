using System;
using System.Collections.Generic;
using System.Text;
using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Application.Interfaces.Repository
{
 
    public interface ISecretaryRepository
    {
        Task<Secretary?> GetByUsernameAsync(string username);

        Task<bool> ExistsByUsernameAsync(string username);

        Task AddAsync(Secretary secretary);
    }
}

