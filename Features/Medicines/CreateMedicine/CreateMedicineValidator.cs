using CRM.API.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.Medicines.CreateMedicine
{
    public class CreateMedicineValidator : AbstractValidator<CreateMedicineRequest>
    {
        public CreateMedicineValidator(AppDbContext db)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200)
                .MustAsync(async (name, ct) => !await db.Medicines.AnyAsync(m => m.Name == name, ct))
                .WithMessage("Medicine with this name already exists.");

            RuleFor(x => x.Price).GreaterThan(0);
        }
    }
}
