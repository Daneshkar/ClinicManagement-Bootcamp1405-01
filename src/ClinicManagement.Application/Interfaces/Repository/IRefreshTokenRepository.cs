using ClinicManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicManagement.Application.Interfaces.Repository
{
    public interface IRefreshTokenRepository
    {
        public Task AddAsync(RefreshToken refreshToken);
        public Task<RefreshToken?> GetByTokenAsync(string token);
        public Task UpdateAsync(RefreshToken refreshToken);
        Task RevokeAllForUserAsync(string userIdentifier);

    }
}
