using Blog.Application.DTOs;
using FluentValidation;

namespace Blog.Application.Validators;

public class CreateAuthorDtoValidator : AbstractValidator<CreateAuthorDto>
{
    public CreateAuthorDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Bio)
            .MaximumLength(1000);
    }
}