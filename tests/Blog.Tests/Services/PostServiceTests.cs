/* using AutoMapper;
using Blog.Application.DTOs;
using Blog.Application.Mappings;
using Blog.Application.Services;
using Blog.Application.Validators;
using Blog.Domain.Entities;
using Blog.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using Moq;
using Xunit;

namespace Blog.Tests.Services;

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _postRepoMock;
    private readonly Mock<IRepository<Tag>> _tagRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePostDto> _createValidator;
    private readonly IValidator<UpdatePostDto> _updateValidator;
    private readonly PostService _service;
    private readonly Mock<IAuthorRepository> _authorRepoMock;
    public PostServiceTests()
    {
        _authorRepoMock = new Mock<IAuthorRepository>();
        _postRepoMock = new Mock<IPostRepository>();
        _tagRepoMock = new Mock<IRepository<Tag>>();
        _uowMock = new Mock<IUnitOfWork>();
       
        // Gerçek AutoMapper kullan (test edilmek istenen şey değil)
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        // Gerçek validator
        _createValidator = new CreatePostDtoValidator();
        _updateValidator = new UpdatePostDtoValidator();

        _service = new PostService(
        _authorRepoMock.Object,    // 1. authorRepo
        _postRepoMock.Object,      // 2. postRepo
        _uowMock.Object,           // 3. uow
        _mapper,                   // 4. mapper
        _createValidator,          // 5. createValidator
        _updateValidator,          // 6. updateValidator
        _tagRepoMock.Object);      // 7. tagRepo
        _authorRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Author { Id = 1, Name = "Test Author" });
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var dto = new CreatePostDto(
            Title: "Test Post Title",
            Content: "This is a test content with enough length to pass validation.",
            AuthorId: 1,
            IsPublished: true,
            Tags: null);

        _postRepoMock
            .Setup(r => r.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post?)null);

        _postRepoMock
            .Setup(r => r.GetByIdWithRelationsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Post
            {
                Id = 1,
                Title = dto.Title,
                Content = dto.Content,
                Slug = "test-post-title",
                AuthorId = 1,
                Author = new Author { Id = 1, Name = "Test Author" }
            });

        // Act
        var result = await _service.CreateAsync(dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be(dto.Title);
        _postRepoMock.Verify(r => r.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithShortTitle_ReturnsValidationError()
    {
        // Arrange
        var dto = new CreatePostDto(
            Title: "Hi",  // Too short
            Content: "Content with enough length here...",
            AuthorId: 1,
            IsPublished: false,
            Tags: null);

        // Act
        var result = await _service.CreateAsync(dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("at least 5 characters"));
        _postRepoMock.Verify(r => r.AddAsync(It.IsAny<Post>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateSlug_ReturnsFailure()
    {
        // Arrange
        var dto = new CreatePostDto(
            Title: "Existing Title",
            Content: "This is a test content with enough length.",
            AuthorId: 1,
            IsPublished: false,
            Tags: null);

        _postRepoMock
            .Setup(r => r.GetBySlugAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Post { Id = 99, Slug = "existing-title" });

        // Act
        var result = await _service.CreateAsync(dto, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors[0].Should().Contain("already exists");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _postRepoMock
            .Setup(r => r.GetByIdWithRelationsAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post?)null);

        // Act
        var result = await _service.GetByIdAsync(999, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
} */