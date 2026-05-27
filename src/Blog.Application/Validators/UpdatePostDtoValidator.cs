using Blog.Application.DTOs;
using FluentValidation;

namespace Blog.Application.Validators;

public class UpdatePostDtoValidator : AbstractValidator<UpdatePostDto>
{
    public UpdatePostDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().MinimumLength(5).MaximumLength(200);

        RuleFor(x => x.Content)
            .NotEmpty().MinimumLength(20);
    }
}