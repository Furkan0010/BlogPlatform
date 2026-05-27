using Blog.Application.DTOs;

namespace Blog.Application.Interfaces;

public interface IAuthorService
{
    Task<AuthorDto?> GetByIdAsync(int id, CancellationToken ct = default);       
    Task<AuthorDto?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken ct = default);
    Task<Result<AuthorDto>> CreateAsync(CreateAuthorDto dto, CancellationToken ct = default);
    Task<Result<bool>> UpdateAsync(int id, UpdateAuthorDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default);
}