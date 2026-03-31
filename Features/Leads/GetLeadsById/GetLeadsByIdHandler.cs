using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.Leads.GetLeadsById
{
    public class GetLeadsByIdHandler(AppDbContext db) : IRequestHandler<GetLeadsByIdQuery, GetLeadsByIdResponse>
    {
        public async Task<GetLeadsByIdResponse> Handle(GetLeadsByIdQuery query, CancellationToken cancellationToken)
        {
            var lead = await db.Leads
                .AsNoTracking()
                .Where(l => l.Id == query.Id)
                .ProjectToType<GetLeadsByIdResponse>()
                .SingleOrDefaultAsync(cancellationToken);

            if (lead == null)
            {
                throw new BusinessException(
                    LoggingMessages.NotFound,
                    $"Fetching lead with ID {query.Id}",
                    System.Net.HttpStatusCode.NotFound
                );
            }

            return lead;
        }
    }
}