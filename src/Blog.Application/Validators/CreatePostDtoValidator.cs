using Blog.Application.DTOs;
using FluentValidation;

namespace Blog.Application.Validators;

public class CreatePostDtoValidator : AbstractValidator<CreatePostDto>
{
    public CreatePostDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MinimumLength(20).WithMessage("Content must be at least 20 characters.");

        RuleFor(x => x.AuthorId)
            .GreaterThan(0).WithMessage("AuthorId must be a positive number.");

        RuleForEach(x => x.Tags!)
            .NotEmpty().WithMessage("Tag cannot be empty.")
            .MaximumLength(50).WithMessage("Tag cannot exceed 50 characters.")
            .When(x => x.Tags != null);
    }
}