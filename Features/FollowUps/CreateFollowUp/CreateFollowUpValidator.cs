using FluentValidation;

namespace CRM.API.Features.FollowUps.CreateFollowUp;

public class CreateFollowUpValidator : AbstractValidator<CreateFollowUpRequest>
{
    public CreateFollowUpValidator()
    {
        RuleFor(x => x.LeadId)
            .NotEmpty().WithMessage("LeadId is required.");

        RuleFor(x => x.FollowUpDate)
            .NotEmpty().WithMessage("Follow-up date is required.")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .WithMessage("Follow-up date cannot be in the past.");

        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("Source (contact medium) is required.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Priority must be a valid level (Low, Medium, High).");
            
        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
    }
}
