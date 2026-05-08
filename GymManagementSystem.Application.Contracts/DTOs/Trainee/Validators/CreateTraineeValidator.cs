using FluentValidation;
using GymManagementSystem.DTOs.Trainee.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DTOs.Trainee.Validators
{
    public class CreateTraineeValidator : AbstractValidator<CreateTraineeCommand>
    {
        public CreateTraineeValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits.");
            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender.");
            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.UtcNow.Date).WithMessage("Date of birth must be in the past.");
        }
    }
}
