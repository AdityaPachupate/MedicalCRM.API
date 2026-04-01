using CRM.API.Common.Enums;
using CRM.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.FollowUps.GetTodayFollowUps;





public class GetTodayFollowUpsHandler(AppDbContext db)
    : IRequestHandler<GetTodayFollowUpsQuery, List<GetTodayFollowUpsResponse>>
{
    public async Task<List<GetTodayFollowUpsResponse>> Handle(GetTodayFollowUpsQuery query, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var followUps = await db.FollowUps
            .Include(f => f.Lead)
            .Where(f => f.Status == FollowUpStatus.Pending && f.FollowUpDate <= today)
            .Select(f => new GetTodayFollowUpsResponse(
                f.Id,
                f.LeadId,
                f.Lead.Name,
                f.Lead.Phone,
                f.FollowUpDate,
                f.Status,
                f.Priority,
                f.Notes,
                f.FollowUpDate < today
            ))
            .OrderByDescending(f => f.IsOverdue)
            .ThenByDescending(f => f.Priority)
            .ThenBy(f => f.FollowUpDate)
            .ToListAsync(cancellationToken);

        return followUps;
    }
}
