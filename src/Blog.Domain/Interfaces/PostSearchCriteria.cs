namespace Blog.Domain.Interfaces;

public record PostSearchCriteria(
    string? Query = null,
    int? AuthorId = null,
    bool? IsPublished = null,
    int Page = 1,
    int PageSize = 10);