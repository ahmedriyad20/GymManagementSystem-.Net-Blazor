using FluentValidation;
using GymManagementSystem.DTOs.Trainee.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DTOs.Trainee.Validators
{
    public class UpdateTraineeValidator : AbstractValidator<UpdateTraineeCommand>
    {
        public UpdateTraineeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10}$").WithMessage("Phone number must be 10 digits.");
            RuleFor(x => x.PaidAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Paid amount cannot be negative.");
            RuleFor(x => x.RemainingAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Remaining amount cannot be negative.");
            RuleFor(x => x.SubscriptionPlan)
                .NotEmpty().WithMessage("Subscription plan is required.")
                .IsInEnum().WithMessage("Invalid subscription plan.");
            RuleFor(x => x.SubscriptionPeriod)
                .NotEmpty().WithMessage("Subscription period is required.")
                .IsInEnum().WithMessage("Invalid subscription period.");
            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Start date cannot be in the future.");
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");
        }
    }
}
