using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.Medicines.GetMedicines
{
    public class GetMedicinesHandler(AppDbContext db) : IRequestHandler<GetMedicinesQuery, List<GetMedicinesResponse>>
    {
        public async Task<List<GetMedicinesResponse>> Handle(GetMedicinesQuery query, CancellationToken ct)
        {
            return await db.Medicines
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ProjectToType<GetMedicinesResponse>()
                .ToListAsync(ct);
        }
    }
}
