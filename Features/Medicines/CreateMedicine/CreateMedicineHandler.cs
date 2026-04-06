using CRM.API.Domain;
using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using CRM.API.Common.Constants;

namespace CRM.API.Features.Medicines.CreateMedicine
{
    public class CreateMedicineHandler(AppDbContext db, ILogger<CreateMedicineHandler> logger) : IRequestHandler<CreateMedicineCommand, CreateMedicineResponse>
    {
        public async Task<CreateMedicineResponse> Handle(CreateMedicineCommand command, CancellationToken ct)
        {
            var medicine = command.Request.Adapt<Medicine>();
            db.Medicines.Add(medicine);
            await db.SaveChangesAsync(ct);
            logger.LogInformation("{Message}: Medicine {MedicineId} created", LoggingMessages.ResourceCreated, medicine.Id);
            return medicine.Adapt<CreateMedicineResponse>();
        }
    }
}
