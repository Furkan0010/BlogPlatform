namespace Blog.Application.DTOs;

public record PagedResult<T>(
    IEnumerable<T> Data,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}

public record SearchCriteria(
    string? Query = null,
    int? AuthorId = null,
    bool? IsPublished = null,
    int Page = 1,
    int PageSize = 10);