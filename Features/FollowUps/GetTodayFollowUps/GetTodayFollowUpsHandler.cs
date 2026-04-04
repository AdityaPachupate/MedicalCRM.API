using CRM.API.Common.Enums;
using CRM.API.Domain;
using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.FollowUps.GetTodayFollowUps;

public class GetTodayFollowUpsHandler(AppDbContext db)
    : IRequestHandler<GetTodayFollowUpsQuery, List<GetTodayFollowUpsResponse>>
{
    public async Task<List<GetTodayFollowUpsResponse>> Handle(GetTodayFollowUpsQuery query, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var config = new TypeAdapterConfig();
        config.NewConfig<FollowUp, GetTodayFollowUpsResponse>()
            .Map(dest => dest.IsOverdue, src => src.FollowUpDate < today);

        var followUps = await db.FollowUps
            .AsNoTracking()
            .Where(f => f.Status == FollowUpStatus.Pending && f.FollowUpDate <= today)
            .ProjectToType<GetTodayFollowUpsResponse>(config)
            .OrderByDescending(f => f.IsOverdue)
            .ThenByDescending(f => f.Priority)
            .ThenBy(f => f.FollowUpDate)
            .ToListAsync(cancellationToken);

        return followUps;
    }
}
