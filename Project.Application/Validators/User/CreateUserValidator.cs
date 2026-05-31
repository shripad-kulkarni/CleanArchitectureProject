using FluentValidation;
using Project.Application.DTOs.User;

namespace Project.Application.Validators.User
{
    public class CreateUserValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(20);

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .Must(dob => dob < DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .Must(g => new[] { "Male", "Female", "Other" }.Contains(g))
                .WithMessage("Gender must be Male, Female, or Other.");

            RuleFor(x => x.Street).NotEmpty().WithMessage("Street is required.").MaximumLength(200);
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required.").MaximumLength(100);
            RuleFor(x => x.State).NotEmpty().WithMessage("State is required.").MaximumLength(100);
            RuleFor(x => x.PinCode).NotEmpty().WithMessage("Pin code is required.").MaximumLength(20);

            RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }
}
