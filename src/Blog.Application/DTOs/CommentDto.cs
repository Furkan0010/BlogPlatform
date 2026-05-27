namespace Blog.Application.DTOs;

public record CommentDto(
    int Id,
    string AuthorName,
    string Content,
    DateTime CreatedAt);

public record CreateCommentDto(
    string AuthorName,
    string Content);