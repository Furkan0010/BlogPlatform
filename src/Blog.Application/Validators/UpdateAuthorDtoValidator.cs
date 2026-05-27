using Blog.Application.DTOs;
using FluentValidation;

namespace Blog.Application.Validators;

public class UpdateAuthorDtoValidator : AbstractValidator<UpdateAuthorDto>
{
    public UpdateAuthorDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(200);

        RuleFor(x => x.Bio)
            .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters.");
    }
}