using CRM.API.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System;

namespace CRM.API.Features.Packages.DeletePackage
{
    public class DeletePackageEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("/packages/{id:guid}", async (
                Guid id,
                [FromQuery] bool isPermanent,
                IMediator mediator,
                CancellationToken cancellationToken)
                =>
                Results.Ok(await mediator.Send(new DeletePackageCommand(id, isPermanent), cancellationToken)))
            .WithName("DeletePackage")
            .WithTags("Packages")
            .Produces<DeletePackageResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithSummary("Delete a package (soft or permanent)");
        }
    }
}
