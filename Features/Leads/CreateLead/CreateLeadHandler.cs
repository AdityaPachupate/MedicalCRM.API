using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.API.Domain;
using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;

namespace CRM.API.Features.Leads.CreateLead
{
    public class CreateLeadHandler(AppDbContext db) : IRequestHandler<CreateLeadCommand, CreateLeadResponse>
    {
        public async Task<CreateLeadResponse> Handle(CreateLeadCommand command, CancellationToken cancellationToken)
        {
            Lead lead = command.Request.Adapt<Lead>();
            await db.Leads.AddAsync(lead);
            await db.SaveChangesAsync(cancellationToken);
            return lead.Adapt<CreateLeadResponse>();
        }
    }
}