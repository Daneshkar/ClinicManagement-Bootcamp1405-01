using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Persistence; // Ensure this matches your ApplicationDbContext namespace
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ClinicDbContext _context;

    public RefreshTokenRepository(ClinicDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _context.Set<RefreshToken>().AddAsync(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(r => r.Token == token);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.Set<RefreshToken>().Update(refreshToken);
        await _context.SaveChangesAsync();
    }


    public async Task RevokeAllForUserAsync(string userIdentifier)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.UserIdentifier == userIdentifier && !x.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Revoke();
        }

        await _context.SaveChangesAsync();
    }


}