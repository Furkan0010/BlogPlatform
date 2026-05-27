using Blog.Application.Helpers;
using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Persistence;

public static class BlogDbSeeder
{
    public static async Task SeedAsync(BlogDbContext context)
    {
        // Migration'ları uygula
        await context.Database.MigrateAsync();

        // Zaten data var mı?
        if (await context.Authors.AnyAsync()) return;

        // Authors
        var authors = new List<Author>
        {
            new() { Name = "Ahmet Yılmaz", Email = "ahmet@blog.com", Bio = "Backend developer" },
            new() { Name = "Ayşe Demir", Email = "ayse@blog.com", Bio = "Full-stack developer" },
            new() { Name = "Mehmet Kaya", Email = "mehmet@blog.com", Bio = "DevOps engineer" }
        };
        await context.Authors.AddRangeAsync(authors);
        await context.SaveChangesAsync();

        // Tags
        var tags = new List<Tag>
        {
            new() { Name = "csharp" }, new() { Name = "dotnet" },
            new() { Name = "web" }, new() { Name = "api" },
            new() { Name = "database" }, new() { Name = "devops" }
        };
        await context.Tags.AddRangeAsync(tags);
        await context.SaveChangesAsync();

        // Posts
        var posts = new List<Post>
        {
            new()
            {
                Title = "Clean Architecture Nedir?",
                Content = "Clean architecture, kod sorumluluklarını ayırmanın bir yoludur...",
                Slug = SlugHelper.Generate("Clean Architecture Nedir?"),
                AuthorId = authors[0].Id,
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-10),
                PostTags = new List<PostTag>
                {
                    new() { TagId = tags[0].Id }, new() { TagId = tags[1].Id }
                }
            },
            new()
            {
                Title = "EF Core ile N+1 Problemi",
                Content = "Entity Framework Core'da yaygın bir performans tuzağı olan N+1 problemi...",
                Slug = SlugHelper.Generate("EF Core ile N+1 Problemi"),
                AuthorId = authors[1].Id,
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-5),
                PostTags = new List<PostTag>
                {
                    new() { TagId = tags[1].Id }, new() { TagId = tags[4].Id }
                }
            },
            new()
            {
                Title = "Docker ile .NET Deployment",
                Content = "Bu yazıda .NET 6 uygulamasını Docker container'a nasıl paketleyeceğimizi...",
                Slug = SlugHelper.Generate("Docker ile .NET Deployment"),
                AuthorId = authors[2].Id,
                IsPublished = false,
                PostTags = new List<PostTag>
                {
                    new() { TagId = tags[1].Id }, new() { TagId = tags[5].Id }
                }
            }
        };
        await context.Posts.AddRangeAsync(posts);
        await context.SaveChangesAsync();

        // Comments
        var comments = new List<Comment>
        {
            new() { PostId = posts[0].Id, AuthorName = "Okuyucu 1", Content = "Çok faydalı yazı, teşekkürler!" },
            new() { PostId = posts[0].Id, AuthorName = "Okuyucu 2", Content = "Domain katmanı için örnek verebilir misiniz?" },
            new() { PostId = posts[1].Id, AuthorName = "Okuyucu 3", Content = "N+1'i ilk defa böyle net anladım." }
        };
        await context.Comments.AddRangeAsync(comments);
        await context.SaveChangesAsync();
    }
}