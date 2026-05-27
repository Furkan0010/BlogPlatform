using AutoMapper;
using Blog.Application.DTOs;
using Blog.Application.Helpers;
using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Blog.Domain.Interfaces;
using FluentValidation;

namespace Blog.Application.Services;

public class PostService : IPostService
{
    private readonly IAuthorRepository _authorRepo;
    private readonly IPostRepository _postRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePostDto> _createValidator;
    private readonly IValidator<UpdatePostDto> _updateValidator;
    private readonly IRepository<Tag> _tagRepo;

    public PostService(
        IAuthorRepository authorRepo,
        IPostRepository postRepo,
        IUnitOfWork uow,
        IMapper mapper,
        IValidator<CreatePostDto> createValidator,
        IValidator<UpdatePostDto> updateValidator,
        IRepository<Tag> tagRepo)
    {
        _authorRepo = authorRepo;
        _postRepo = postRepo;
        _uow = uow;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _tagRepo = tagRepo;
    }

    public async Task<PostDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var post = await _postRepo.GetByIdWithRelationsAsync(id, ct);
        return post == null ? null : _mapper.Map<PostDetailDto>(post);
    }

    public async Task<PostDetailDto?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var post = await _postRepo.GetBySlugAsync(slug, ct);
        return post == null ? null : _mapper.Map<PostDetailDto>(post);
    }

    public async Task<IEnumerable<PostListDto>> GetAllAsync(CancellationToken ct = default)
    {
       var posts = await _postRepo.GetAllWithAuthorAsync(ct);
       return _mapper.Map<IEnumerable<PostListDto>>(posts);
    }

    public async Task<Result<PostDetailDto>> CreateAsync(CreatePostDto dto, CancellationToken ct = default)
    {
        // 1. Validation
        var validation = await _createValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return Result<PostDetailDto>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).ToArray());
        
        // 2. Author gerçekten var mı?
        var author = await _authorRepo.GetByIdAsync(dto.AuthorId, ct);
        if (author == null)
        return Result<PostDetailDto>.Failure($"Author {dto.AuthorId} not found.");

        // 2. Slug oluştur ve duplicate kontrol
        var slug = SlugHelper.Generate(dto.Title);
        var existing = await _postRepo.GetBySlugAsync(slug, ct);
        if (existing != null)
            return Result<PostDetailDto>.Failure($"A post with slug '{slug}' already exists.");

        // 3. Entity oluştur
        var post = new Post
        {
            Title = dto.Title,
            Content = dto.Content,
            Slug = slug,
            AuthorId = dto.AuthorId,
            IsPublished = dto.IsPublished,
            PublishedAt = dto.IsPublished ? DateTime.UtcNow : null
        };

        if (dto.Tags != null && dto.Tags.Any())
        {
        foreach (var tagName in dto.Tags.Select(t => t.Trim().ToLower()).Distinct())
        {
            if (string.IsNullOrWhiteSpace(tagName)) continue;

            // Var mı, yok mu kontrol et
            var existingTags = await _tagRepo.FindAsync(t => t.Name == tagName, ct);
            var tag = existingTags.FirstOrDefault() ?? new Tag { Name = tagName };

            post.PostTags.Add(new PostTag { Tag = tag });
        }
        }

        // 4. Persist
        await _postRepo.AddAsync(post, ct);
        await _uow.SaveChangesAsync(ct);

        // 5. Tam veriyle döndür
        var created = await _postRepo.GetByIdWithRelationsAsync(post.Id, ct);
        return Result<PostDetailDto>.Success(_mapper.Map<PostDetailDto>(created!));
    }

    public async Task<Result<bool>> UpdateAsync(int id, UpdatePostDto dto, CancellationToken ct = default)
    {
        var validation = await _updateValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return Result<bool>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).ToArray());

        var post = await _postRepo.GetByIdAsync(id, ct);
        if (post == null) return Result<bool>.Failure($"Post {id} not found.");

        post.Title = dto.Title;
        post.Content = dto.Content;
        post.IsPublished = dto.IsPublished;
        if (dto.IsPublished && post.PublishedAt == null)
            post.PublishedAt = DateTime.UtcNow;

        _postRepo.Update(post);
        await _uow.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var post = await _postRepo.GetByIdAsync(id, ct);
        if (post == null) return Result<bool>.Failure($"Post {id} not found.");

        _postRepo.Delete(post);
        await _uow.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    public async Task<PagedResult<PostListDto>> SearchAsync(SearchCriteria criteria, CancellationToken ct = default)
    {
    var domainCriteria = new PostSearchCriteria(criteria.Query, criteria.AuthorId, criteria.IsPublished, criteria.Page, criteria.PageSize);

    var (posts, total) = await _postRepo.SearchAsync(domainCriteria, ct);
    var dtos = _mapper.Map<IEnumerable<PostListDto>>(posts);

    return new PagedResult<PostListDto>(dtos, criteria.Page, criteria.PageSize, total);
    }
}