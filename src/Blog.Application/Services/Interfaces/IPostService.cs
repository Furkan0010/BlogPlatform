using Blog.Application.DTOs;

namespace Blog.Application.Interfaces;

public interface IPostService
{
    Task<PostDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PostDetailDto?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<IEnumerable<PostListDto>> GetAllAsync(CancellationToken ct = default);
    Task<Result<PostDetailDto>> CreateAsync(CreatePostDto dto, CancellationToken ct = default);
    Task<Result<bool>> UpdateAsync(int id, UpdatePostDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
    Task<PagedResult<PostListDto>> SearchAsync(SearchCriteria criteria, CancellationToken ct = default);
}