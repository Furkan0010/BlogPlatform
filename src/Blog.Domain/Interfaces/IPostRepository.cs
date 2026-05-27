using Blog.Domain.Entities;

namespace Blog.Domain.Interfaces;

public interface IPostRepository : IRepository<Post>
{
    Task<IEnumerable<Post>> GetAllWithAuthorAsync(CancellationToken ct = default);
    Task<Post?> GetByIdWithRelationsAsync(int id, CancellationToken ct = default);
    Task<Post?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<(IEnumerable<Post> Data, int TotalCount)> SearchAsync(
    PostSearchCriteria criteria, CancellationToken ct = default);
    
}