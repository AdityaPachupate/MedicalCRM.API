using CRM.API.Common.Constants;
using CRM.API.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.Lookups.AddLookup
{
    public class AddLookupValidator : AbstractValidator<AddLookupCommand>
    {
        public AddLookupValidator(AppDbContext db)
        {
            RuleFor(x => x.Request.Category)
                .NotEmpty().WithMessage("Category is required.")
                .Must(c => LookupCategories.All.Contains(c))
                .WithMessage($"Category must be one of the following: {string.Join(", ", LookupCategories.All)}");

            RuleFor(x => x.Request.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(50).WithMessage("Code cannot exceed 50 characters.");

            RuleFor(x => x.Request.DisplayName)
                .NotEmpty().WithMessage("Display Name is required.")
                .MaximumLength(100).WithMessage("Display Name cannot exceed 100 characters.");

            // Uniqueness check for Code within Category
            RuleFor(x => x.Request)
                .MustAsync(async (req, ct) => 
                    !await db.LookupValues.AnyAsync(l => l.Category == req.Category && l.Code == req.Code, ct))
                .WithMessage(x => $"A lookup value with the Category '{x.Request.Category}' and Code '{x.Request.Code}' already exists.");
        }
    }
}
