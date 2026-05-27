using Blog.Domain.Entities;
using Blog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
private readonly string _dbName = "TestDb_" + Guid.NewGuid();

protected override void ConfigureWebHost(IWebHostBuilder builder)
{
    builder.ConfigureServices(services =>
    {
        // EF Core ile ilgili tüm kayıtları temizle
        var descriptorsToRemove = services
            .Where(d => d.ServiceType == typeof(DbContextOptions<BlogDbContext>)
                     || d.ServiceType == typeof(DbContextOptions)
                     || d.ServiceType == typeof(BlogDbContext)
                     || (d.ServiceType.Namespace != null && d.ServiceType.Namespace.StartsWith("Microsoft.EntityFrameworkCore")))
            .ToList();

        foreach (var descriptor in descriptorsToRemove)
        {
            services.Remove(descriptor);
        }

        // In-memory DB ekle - sabit isim kullan
        services.AddDbContext<BlogDbContext>(options =>
            options.UseInMemoryDatabase(_dbName));
    });

    builder.UseEnvironment("Testing");
}

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task<int> SeedAuthorAsync()
   {
       using var scope = Services.CreateScope();
       var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();

       var existing = await context.Authors.FirstOrDefaultAsync();
       if (existing != null) return existing.Id;

       var author = new Author
       {
           Name = "Test Author",
           Email = "test@example.com",
           Bio = "Test bio"
       };
       await context.Authors.AddAsync(author);
       await context.SaveChangesAsync();
       return author.Id;  // EF SaveChanges sonrası gerçek Id'yi doldurur
    }
}