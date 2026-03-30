namespace CRM.API.Features.Leads.GetLeads
{
    using CRM.API.Common.ExceptionHandling;
    using CRM.API.Domain;
    using CRM.API.Infrastructure.Persistence;
    using Mapster;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class GetLeadsHandler(AppDbContext db) : IRequestHandler<GetLeadsQuery, List<GetLeadsResponse>>
    {
        async Task<List<GetLeadsResponse>> IRequestHandler<GetLeadsQuery, List<GetLeadsResponse>>.Handle(GetLeadsQuery query, CancellationToken cancellationToken)
        {
            IQueryable<Lead> leads = db.Leads.AsQueryable();

            if (query.status.HasValue)
            {
                leads = leads.Where(l => l.Status == query.status.Value);
            }

            if (!string.IsNullOrEmpty(query.source))
            {
                leads = leads.Where(l => l.Source == query.source);
            }

            var result = await leads
             .OrderByDescending(l => l.CreatedAt)
             .Skip((query.PageNumber - 1) * query.PageSize)
             .Take(query.PageSize)
             .ProjectToType<GetLeadsResponse>()
             .ToListAsync(cancellationToken);

            return result;
        }
    }
}
