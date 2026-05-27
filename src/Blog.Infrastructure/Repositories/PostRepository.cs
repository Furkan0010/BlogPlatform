using Blog.Domain.Entities;
using Blog.Domain.Interfaces;
using Blog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Repositories;

public class PostRepository : Repository<Post>, IPostRepository
{
    public PostRepository(BlogDbContext context) : base(context) { }

    public async Task<Post?> GetByIdWithRelationsAsync(int id, CancellationToken ct = default)
        => await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Comments)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Post?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await _dbSet
            .Include(p => p.Author)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Slug == slug, ct);

    public async Task<IEnumerable<Post>> GetAllWithAuthorAsync(CancellationToken ct = default)
    => await _dbSet
        .Include(p => p.Author)
        .AsNoTracking()
        .ToListAsync(ct);

    public async Task<(IEnumerable<Post> Data, int TotalCount)> SearchAsync(PostSearchCriteria criteria, CancellationToken ct = default)
    {
    IQueryable<Post> query = _dbSet.Include(p => p.Author).AsNoTracking();

    if (!string.IsNullOrWhiteSpace(criteria.Query))
    {
        var q = criteria.Query.ToLower();
        query = query.Where(p => p.Title.ToLower().Contains(q) || p.Content.ToLower().Contains(q));
    }

    if (criteria.AuthorId.HasValue)
        query = query.Where(p => p.AuthorId == criteria.AuthorId.Value);

    if (criteria.IsPublished.HasValue)
        query = query.Where(p => p.IsPublished == criteria.IsPublished.Value);

    var total = await query.CountAsync(ct);

    var data = await query
        .OrderByDescending(p => p.CreatedAt)
        .Skip((criteria.Page - 1) * criteria.PageSize)
        .Take(criteria.PageSize)
        .ToListAsync(ct);

    return (data, total);
    }
}