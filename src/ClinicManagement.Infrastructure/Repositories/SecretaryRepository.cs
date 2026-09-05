using ClinicManagement.Application.Interfaces.Repository;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagement.Infrastructure.Repositories;

public class SecretaryRepository : ISecretaryRepository
{
    private readonly ClinicDbContext _context;

    public SecretaryRepository(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<Secretary?> GetByUsernameAsync(string username)
    {
        return await _context.Secretaries
            .FirstOrDefaultAsync(s => s.UserName == username);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Secretaries
            .AnyAsync(s => s.UserName == username);
    }

    public async Task AddAsync(Secretary secretary)
    {
        await _context.Secretaries.AddAsync(secretary);
        await _context.SaveChangesAsync();
    }
}