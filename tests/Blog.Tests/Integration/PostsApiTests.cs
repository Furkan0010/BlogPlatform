using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Blog.Application.DTOs;
using Blog.Domain.Entities;
using Blog.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blog.Tests.Integration;

public class PostsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PostsApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithEmptyListInitially()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/posts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var posts = await response.Content.ReadFromJsonAsync<List<PostListDto>>(_jsonOptions);
        posts.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task CreatePost_WithValidData_Returns201()
    {
        // Arrange
        await _factory.ResetDatabaseAsync();
        var authorId = await _factory.SeedAuthorAsync();  // önce bir author yarat

        var dto = new CreatePostDto(
            Title: "Integration Test Post",
            Content: "This is a test content with enough length for validation.",
            AuthorId: authorId,
            IsPublished: true,
            Tags: new[] { "testing", "integration" });

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/posts", dto);
       
        // Geçici teşhis
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {response.StatusCode}, Body: {body}");
        
        Console.WriteLine($"ICERIK VERISI: {dto.AuthorId}, {dto.Content}, {dto.IsPublished}, {dto.Tags}, {dto.Title}");
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var created = await response.Content.ReadFromJsonAsync<PostDetailDto>(_jsonOptions);
        created.Should().NotBeNull();
        created!.Title.Should().Be(dto.Title);
        created.Tags.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPost_WithInvalidId_Returns404()
    {
        var response = await _client.GetAsync("/api/v1/posts/9999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreatePost_WithInvalidData_Returns400()
    {
        var dto = new CreatePostDto(
            Title: "Hi",  // Too short
            Content: "Short",  // Too short
            AuthorId: 1,
            IsPublished: false,
            Tags: null);

        var response = await _client.PostAsJsonAsync("/api/v1/posts", dto);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}