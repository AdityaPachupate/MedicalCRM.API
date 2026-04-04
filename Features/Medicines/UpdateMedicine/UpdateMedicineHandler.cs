using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CRM.API.Features.Medicines.UpdateMedicine
{
    public class UpdateMedicineHandler(AppDbContext db) : IRequestHandler<UpdateMedicineCommand, UpdateMedicineResponse>
    {
        public async Task<UpdateMedicineResponse> Handle(UpdateMedicineCommand command, CancellationToken ct)
        {
            var medicine = await db.Medicines
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(m => m.Id == command.Request.Id, ct);

            if (medicine == null)
            {
                throw new BusinessException(
                    LoggingMessages.NotFound,
                    $"Fetching medicine with ID {command.Request.Id}",
                    HttpStatusCode.NotFound
                );
            }

            if (!string.IsNullOrWhiteSpace(command.Request.Name))
            {
                medicine.Name = command.Request.Name;
            }

            if (command.Request.Price.HasValue)
            {
                medicine.Price = command.Request.Price.Value;
            }

            if (command.Request.IsActive.HasValue)
            {
                medicine.IsActive = command.Request.IsActive.Value;
            }

            await db.SaveChangesAsync(ct);
            return medicine.Adapt<UpdateMedicineResponse>();
        }
    }
}
