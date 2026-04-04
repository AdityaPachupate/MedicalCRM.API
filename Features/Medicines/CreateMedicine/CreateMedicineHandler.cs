using CRM.API.Domain;
using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;

namespace CRM.API.Features.Medicines.CreateMedicine
{
    public class CreateMedicineHandler(AppDbContext db) : IRequestHandler<CreateMedicineCommand, CreateMedicineResponse>
    {
        public async Task<CreateMedicineResponse> Handle(CreateMedicineCommand command, CancellationToken ct)
        {
            var medicine = command.Request.Adapt<Medicine>();
            db.Medicines.Add(medicine);
            await db.SaveChangesAsync(ct);
            return medicine.Adapt<CreateMedicineResponse>();
        }
    }
}
