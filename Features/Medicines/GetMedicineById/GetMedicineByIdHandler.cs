using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.API.Features.Medicines.GetMedicineById
{
    public class GetMedicineByIdHandler(AppDbContext db) : IRequestHandler<GetMedicineByIdQuery, GetMedicineByIdResponse>
    {
        public async Task<GetMedicineByIdResponse> Handle(GetMedicineByIdQuery query, CancellationToken ct)
        {
            var medicine = await db.Medicines
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == query.Id, ct);

            if (medicine == null)
            {
                throw new BusinessException(
                    LoggingMessages.NotFound,
                    $"Medicine with ID {query.Id} not found.",
                    System.Net.HttpStatusCode.NotFound
                );
            }

            return medicine.Adapt<GetMedicineByIdResponse>();
        }
    }
}
