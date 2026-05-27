using Blog.Domain.Entities;
using Blog.Domain.Interfaces;
using Blog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Repositories;

public class AuthorRepository : Repository<Author>, IAuthorRepository
{
    public AuthorRepository(BlogDbContext context) : base(context) { }

    public async Task<Author?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email, ct);
}