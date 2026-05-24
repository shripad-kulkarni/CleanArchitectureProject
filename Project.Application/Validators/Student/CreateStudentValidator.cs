using FluentValidation;
using Project.Application.DTOs.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Validators.Student
{
    public class CreateStudentValidator : AbstractValidator<CreateStudentDto>
    {
        public CreateStudentValidator()
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
                .Matches(@"^[6-9]\d{9}$").WithMessage("Invalid Indian phone number.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .Must(dob => dob <= DateOnly.FromDateTime(DateTime.Today).AddYears(-3))
                .WithMessage("Student must be at least 3 years old.");

            RuleFor(x => x.Gender)
                .NotEmpty()
                .Must(g => new[] { "Male", "Female", "Other" }.Contains(g))
                .WithMessage("Gender must be Male, Female, or Other.");

            RuleFor(x => x.RollNumber)
                .NotEmpty().WithMessage("Roll number is required.");

            RuleFor(x => x.ClassName)
                .NotEmpty().WithMessage("Class is required.");

            RuleFor(x => x.AcademicYear)
                .NotEmpty()
                .Matches(@"^\d{4}-\d{4}$")
                .WithMessage("Academic year must be in format YYYY-YYYY.");

            RuleFor(x => x.ParentName)
                .NotEmpty().WithMessage("Parent/Guardian name is required.");

            RuleFor(x => x.ParentPhone)
                .NotEmpty()
                .Matches(@"^[6-9]\d{9}$").WithMessage("Invalid parent phone number.");
        }
    }
}
