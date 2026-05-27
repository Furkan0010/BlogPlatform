using AutoMapper;
using Blog.Application.DTOs;
using Blog.Application.Interfaces;
using Blog.Domain.Entities;
using Blog.Domain.Interfaces;
using FluentValidation;

namespace Blog.Application.Services;

public class CommentService : ICommentService
{
    private readonly IPostRepository _postRepo;
    private readonly IRepository<Comment> _commentRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateCommentDto> _validator;

    public CommentService(
        IPostRepository postRepo,
        IRepository<Comment> commentRepo,
        IUnitOfWork uow,
        IMapper mapper,
        IValidator<CreateCommentDto> validator)
    {
        _postRepo = postRepo;
        _commentRepo = commentRepo;
        _uow = uow;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<Result<CommentDto>> AddCommentAsync(int postId, CreateCommentDto dto, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(dto, ct);
        if (!validation.IsValid)
            return Result<CommentDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToArray());

        var post = await _postRepo.GetByIdAsync(postId, ct);
        if (post == null)
            return Result<CommentDto>.Failure($"Post {postId} not found.");

        var comment = new Comment
        {
            AuthorName = dto.AuthorName,
            Content = dto.Content,
            PostId = postId
        };

        await _commentRepo.AddAsync(comment, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<CommentDto>.Success(_mapper.Map<CommentDto>(comment));
    }

    public async Task<Result<bool>> DeleteCommentAsync(int id, CancellationToken ct = default)
    {
        var comment = await _commentRepo.GetByIdAsync(id, ct);
        if (comment == null) return Result<bool>.Failure($"Comment {id} not found.");

        _commentRepo.Delete(comment);
        await _uow.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }
}