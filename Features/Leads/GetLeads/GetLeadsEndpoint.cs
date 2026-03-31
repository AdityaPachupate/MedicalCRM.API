using CRM.API.Common.Interfaces;
using MediatR;

namespace CRM.API.Features.Leads.GetLeads;

public class GetLeadsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/leads", async (
            [AsParameters] GetLeadsQuery query,
            IMediator mediator,
            CancellationToken cancellationToken) =>
            Results.Ok(await mediator.Send(query, cancellationToken)
            )
        );
    }
}