using CRM.API.Common.Interfaces;
using MediatR;

namespace CRM.API.Features.Medicines.GetMedicines
{
    public class GetMedicinesEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicines", async (IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new GetMedicinesQuery(), ct);
                return Results.Ok(result);
            })
            .WithName("GetMedicines")
            .WithTags("Medicines")
            .WithSummary("Get all active medicines");
        }
    }
}
