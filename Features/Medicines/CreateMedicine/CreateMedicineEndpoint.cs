using CRM.API.Common.Interfaces;
using MediatR;

namespace CRM.API.Features.Medicines.CreateMedicine
{
    public class CreateMedicineEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/medicines", async (CreateMedicineRequest request, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new CreateMedicineCommand(request), ct);
                return Results.Created($"/medicines/{result.Id}", result);
            })
            .WithName("CreateMedicine")
            .WithTags("Medicines")
            .WithSummary("Add a new medicine to the catalog");
        }
    }
}
