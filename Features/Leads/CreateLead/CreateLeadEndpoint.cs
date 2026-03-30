using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.API.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace CRM.API.Features.Leads.CreateLead
{
    public class CreateLeadEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/create-lead", async (
                CreateLeadRequest request, IMediator mediator, CancellationToken cancellationToken
            ) =>
            {
                var result = await mediator.Send(new CreateLeadCommand(request), cancellationToken);
                return Results.Created($"/api/create-lead/{result.Id}", result);
            });
        }
    }
}