using AutoMapper;
using Blog.Application.DTOs;
using Blog.Application.Helpers;
using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Blog.Domain.Interfaces;
using FluentValidation;

namespace Blog.Application.Services;

public class AuthorService  : IAuthorService
{
    private readonly IAuthorRepository _authorRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateAuthorDto> _createValidator;
    private readonly IValidator<UpdateAuthorDto> _updateValidator;

    public AuthorService (
        IAuthorRepository authorRepo,
        IUnitOfWork uow,
        IMapper mapper,
        IValidator<CreateAuthorDto> createValidator,
        IValidator<UpdateAuthorDto> updateValidator)
    {
        _authorRepo = authorRepo;
        _uow = uow;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }


    public async Task<AuthorDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var author = await _authorRepo.GetByIdAsync(id, ct);
        return author == null ? null : _mapper.Map<AuthorDto>(author);
    }
    public async Task<AuthorDto?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var author = await _authorRepo.GetByEmailAsync(email, ct);
        return author == null ? null : _mapper.Map<AuthorDto>(author);
    }

    public async Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken ct = default)
    {
        var author = await _authorRepo.GetAllAsync(ct);
        return _mapper.Map<IEnumerable<AuthorDto>>(author);
    }

    public async Task<Result<AuthorDto>> CreateAsync(CreateAuthorDto dto, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return Result<AuthorDto>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).ToArray());

        var existing = await _authorRepo.GetByEmailAsync(dto.Email, ct);
        if (existing != null)
        return Result<AuthorDto>.Failure($"An author with email '{dto.Email}' already exists.");

        var author = new Author
        {
            Name = dto.Name,
            Email = dto.Email,
            Bio = dto.Bio,
            CreatedAt = DateTime.UtcNow
        };

        await _authorRepo.AddAsync(author, ct);
        await _uow.SaveChangesAsync(ct);

        var created = await _authorRepo.GetByIdAsync(author.Id, ct);
        return Result<AuthorDto>.Success(_mapper.Map<AuthorDto>(created!));
    }

    public async Task<Result<bool>> UpdateAsync(int id, UpdateAuthorDto dto, CancellationToken ct = default)
    {
         var validation = await _updateValidator.ValidateAsync(dto, ct);
         if (!validation.IsValid)
        return Result<bool>.Failure(
        validation.Errors.Select(e => e.ErrorMessage).ToArray());
        
        var author = await _authorRepo.GetByIdAsync(id, ct);
        if (author == null) return Result<bool>.Failure($"Author {id} not found.");

        var existing = await _authorRepo.GetByEmailAsync(dto.Email, ct);
        if (existing != null && existing.Id != id)
        return Result<bool>.Failure($"An author with email '{dto.Email}' already exists.");

        author.Name = dto.Name;
        author.Email = dto.Email;
        author.Bio = dto.Bio;

        _authorRepo.Update(author);
        await _uow.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var author = await _authorRepo.GetByIdAsync(id, ct);
        if (author == null) return Result<bool>.Failure($"Author {id} not found.");

        _authorRepo.Delete(author);
        await _uow.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}