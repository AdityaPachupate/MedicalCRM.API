using CRM.API.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CRM.API.Features.Rejoin.DeleteRejoin;

public class DeleteRejoinEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/rejoins/{id:guid}", async (Guid id, [FromQuery] bool isPermanent, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new DeleteRejoinCommand(id, isPermanent), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("DeleteRejoin")
        .WithTags("Rejoins")
        .Produces<DeleteRejoinResponse>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .WithSummary("Soft delete a custom rejoin record");
    }
}
