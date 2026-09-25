using FluentValidation;
using WebApiApp.Dto;

namespace WebApiApp.Validators;

internal sealed class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        // Name Validation
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required")
            .Must(BeValidName).WithMessage("Name must contain only letters")
            .MaximumLength(50).WithMessage("Name must be at most 50 characters long");

        // Email Validation
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is invalid");
        
        // Password Validation
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long");
        
    }

    private static bool BeValidName(string name)
    {
        return !string.IsNullOrWhiteSpace(name)
            && name.All(c => char.IsLetter(c) || c == ' ');
    }
}