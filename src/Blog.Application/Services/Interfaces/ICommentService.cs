using Blog.Application.DTOs;

namespace Blog.Application.Interfaces;

public interface ICommentService
{
    Task<Result<CommentDto>> AddCommentAsync(int postId, CreateCommentDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteCommentAsync(int id, CancellationToken ct = default);
}