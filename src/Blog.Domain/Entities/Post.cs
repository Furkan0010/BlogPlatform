namespace Blog.Domain.Entities;

public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }

    // Foreign Key
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;

    // Navigations (sonraki adımlarda dolduracağız)
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}