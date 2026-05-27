namespace Blog.Domain.Entities;

public class Comment : BaseEntity
{
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    // FK
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;
}