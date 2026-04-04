using MediatR;

namespace CRM.API.Features.Medicines.GetMedicines
{
    public record GetMedicinesResponse(Guid Id, string Name, decimal Price, DateTime CreatedAt);
    public record GetMedicinesQuery() : IRequest<List<GetMedicinesResponse>>;
}
