using CRM.API.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CRM.API.Features.Leads.DeleteLead
{
    public class DeleteLeadEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("/leads/{id:guid}", async (
                Guid id, IMediator mediator, CancellationToken cancellationToken
            ) =>
            {
                var result = await mediator.Send(new DeleteLeadCommand(new DeleteLeadRequest(id)), cancellationToken);
                return Results.Ok(result);
            })
            .WithName("DeleteLead")
            .WithTags("Leads")
            .Produces<DeleteLeadResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithSummary("Delete a lead by ID");
        }
    }
}