using Blog.Application.Interfaces;
using Blog.Application.Mappings;
using Blog.Application.Services;
using Blog.Application.Validators;
using Blog.Domain.Interfaces;
using Blog.Infrastructure.Persistence;
using Blog.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === DOMAIN katmanı bağımlılık almaz ===

// === INFRASTRUCTURE ===
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
           .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BlogDbContext>());
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICommentService, CommentService>();

// === APPLICATION ===
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<CreatePostDtoValidator>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

// === API ===
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BlogPlatform API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// === Pipeline ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlogPlatform API v1"));
}

app.UseMiddleware<Blog.Api.Middleware.ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    await BlogDbSeeder.SeedAsync(context);
}

app.Run();

public partial class Program { } // Integration test'ler için