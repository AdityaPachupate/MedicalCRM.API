namespace CRM.API.Features.Leads.GetLeads
{
    using CRM.API.Common.ExceptionHandling;
    using CRM.API.Common.Models;
    using CRM.API.Domain;
    using CRM.API.Infrastructure.Persistence;
    using Mapster;
    using MediatR;
    using Microsoft.EntityFrameworkCore;

    public class GetLeadsHandler(AppDbContext db) : IRequestHandler<GetLeadsQuery, PagedResult<GetLeadsResponse>>
    {
        async Task<PagedResult<GetLeadsResponse>> IRequestHandler<GetLeadsQuery, PagedResult<GetLeadsResponse>>.Handle(GetLeadsQuery query, CancellationToken cancellationToken)
        {
            IQueryable<Lead> queryableLeads = db.Leads.AsQueryable();

            if (query.status.HasValue)
            {
                queryableLeads = queryableLeads.Where(l => l.Status == query.status.Value);
            }

            if (!string.IsNullOrEmpty(query.source))
            {
                queryableLeads = queryableLeads.Where(l => l.Source == query.source);
            }

            var totalCount = await queryableLeads.CountAsync(cancellationToken);

            var items = await queryableLeads
             .OrderByDescending(l => l.CreatedAt)
             .Skip((query.PageNumber - 1) * query.PageSize)
             .Take(query.PageSize)
             .ProjectToType<GetLeadsResponse>()
             .ToListAsync(cancellationToken);

            return new PagedResult<GetLeadsResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
    }
}
