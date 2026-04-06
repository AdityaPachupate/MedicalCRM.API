using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.API.Features.Lookups.GetLookups
{
    public class GetLookupsHandler(AppDbContext db) : IRequestHandler<GetLookupsQuery, List<GetLookupsResponse>>
    {
        public async Task<List<GetLookupsResponse>> Handle(GetLookupsQuery query, CancellationToken cancellationToken)
        {
            var queryable = db.LookupValues
                .AsNoTracking()
                .Where(l => !l.IsDeleted);

            if (!string.IsNullOrEmpty(query.Category))
            {
                queryable = queryable.Where(l => l.Category == query.Category);
            }

            var items = await queryable
                .OrderBy(l => l.Category)
                .ThenBy(l => l.DisplayName)
                .ProjectToType<GetLookupsResponse>()
                .ToListAsync(cancellationToken);

            return items;
        }
    }
}
