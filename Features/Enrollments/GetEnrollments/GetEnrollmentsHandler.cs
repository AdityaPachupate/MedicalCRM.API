using CRM.API.Common.Models;
using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.Enrollments.GetEnrollments
{
    public class GetEnrollmentsHandler(AppDbContext db) : IRequestHandler<GetEnrollmentsQuery, PagedResult<GetEnrollmentsResponse>>
    {
        public async Task<PagedResult<GetEnrollmentsResponse>> Handle(GetEnrollmentsQuery query, CancellationToken cancellationToken)
        {
            var queryable = query.IsTrash
                ? db.Enrollments.IgnoreQueryFilters().Where(e => e.IsDeleted)
                : db.Enrollments.AsQueryable();

            var totalCount = await queryable.CountAsync(cancellationToken);

            var items = await queryable
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ProjectToType<GetEnrollmentsResponse>()
                .ToListAsync(cancellationToken);

            return new PagedResult<GetEnrollmentsResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }
    }
}
