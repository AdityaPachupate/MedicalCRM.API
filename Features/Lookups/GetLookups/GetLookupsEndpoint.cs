using CRM.API.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;

namespace CRM.API.Features.Lookups.GetLookups
{
    public class GetLookupsEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/lookups", async (
                [FromQuery] string? category,
                IMediator mediator,
                CancellationToken cancellationToken)
                =>
                Results.Ok(await mediator.Send(new GetLookupsQuery(category), cancellationToken)))
            .WithName("GetLookups")
            .WithTags("Lookups")
            .Produces<List<GetLookupsResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithSummary("Get a list of lookup values");
        }
    }
}
