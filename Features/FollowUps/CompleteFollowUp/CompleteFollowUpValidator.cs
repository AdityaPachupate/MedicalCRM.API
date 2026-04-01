using FluentValidation;
using CRM.API.Common.Enums;

namespace CRM.API.Features.FollowUps.CompleteFollowUp;

public class CompleteFollowUpValidator : AbstractValidator<CompleteFollowUpRequest>
{
    public CompleteFollowUpValidator()
    {
        RuleFor(x => x.FollowUpId)
            .NotEmpty().WithMessage("FollowUpId is required.");

        RuleFor(x => x.Outcome)
            .IsInEnum().WithMessage("A valid outcome must be provided.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");

        RuleFor(x => x.NextFollowUpDate)
            .Must(date => !date.HasValue || date.Value >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Next follow-up date cannot be in the past.");

        RuleFor(x => x.NewLeadStatus)
            .Must(status => !status.HasValue || status.Value != LeadStatus.New)
            .WithMessage("Lead status cannot be reset to 'New' during follow-up completion.");
    }
}
