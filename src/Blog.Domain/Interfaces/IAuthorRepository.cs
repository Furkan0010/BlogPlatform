using Blog.Domain.Entities;

namespace Blog.Domain.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<Author?> GetByEmailAsync(string email, CancellationToken ct = default);
}