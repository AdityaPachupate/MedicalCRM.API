using CRM.API.Common.Constants;
using CRM.API.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.Leads.CreateLead
{
    public class CreateLeadValidator : AbstractValidator<CreateLeadRequest>
    {
        public CreateLeadValidator(AppDbContext db)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?[0-9]{7,15}$");
            RuleFor(x => x.Status).IsInEnum();

            // Validate Source exists as an active lookup value
            RuleFor(x => x.Source)
                .NotEmpty()
                .MustAsync(async (source, ct) =>
                    await db.LookupValues.AnyAsync(l =>
                        l.Category == LookupCategories.LeadSource &&
                        l.Code == source && l.IsActive, ct))
                .WithMessage(x => $"'{x.Source}' is not a valid active lead source.");

            // Validate Reason exists as an active lookup value
            RuleFor(x => x.Reason)
                .NotEmpty()
                .MustAsync(async (reason, ct) =>
                    await db.LookupValues.AnyAsync(l =>
                        l.Category == LookupCategories.LeadReason &&
                        l.Code == reason && l.IsActive, ct))
                .WithMessage(x => $"'{x.Reason}' is not a valid active lead reason.");
        }
    }
}
