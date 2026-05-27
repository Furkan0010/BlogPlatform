using Blog.Application.DTOs;
using FluentValidation;

namespace Blog.Application.Validators;

public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(x => x.AuthorName)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Content)
            .NotEmpty().MinimumLength(3).MaximumLength(2000);
    }
}