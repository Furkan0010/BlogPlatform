namespace Blog.Application.DTOs;

public record PostListDto(
    int Id,
    string Title,
    string Slug,
    bool IsPublished,
    DateTime CreatedAt,
    DateTime? PublishedAt,
    string AuthorName);

public record PostDetailDto(
    int Id,
    string Title,
    string Content,
    string Slug,
    bool IsPublished,
    DateTime CreatedAt,
    DateTime? PublishedAt,
    AuthorDto Author,
    IEnumerable<CommentDto> Comments,
    IEnumerable<string> Tags);

public record CreatePostDto(
    string Title,
    string Content,
    int AuthorId,
    bool IsPublished,
    IEnumerable<string>? Tags);

public record UpdatePostDto(
    string Title,
    string Content,
    bool IsPublished,
    IEnumerable<string>? Tags);