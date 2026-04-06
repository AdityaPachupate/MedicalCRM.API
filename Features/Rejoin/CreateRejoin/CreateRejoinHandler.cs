using CRM.API.Common.Constants;
using CRM.API.Common.Enums;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Common.Interfaces;
using CRM.API.Domain;
using CRM.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Linq;

namespace CRM.API.Features.Rejoin.CreateRejoin;

public class CreateRejoinHandler(AppDbContext db, ILogger<CreateRejoinHandler> logger) 
    : IRequestHandler<CreateRejoinCommand, CreateRejoinResponse>
{
    public async Task<CreateRejoinResponse> Handle(CreateRejoinCommand command, CancellationToken ct)
    {
        // 1. Fetch Related Entities
        var lead = await db.Leads
            .Include(l => l.FollowUps)
            .FirstOrDefaultAsync(l => l.Id == command.LeadId, ct);
            
        if (lead == null)
        {
            logger.LogWarning("{Message}: Fetching Lead with ID {LeadId} not found", LoggingMessages.NotFound, command.LeadId);
            throw new BusinessException(LoggingMessages.NotFound, $"Lead {command.LeadId} not found", HttpStatusCode.NotFound);
        }
        
        var package = await db.Packages.FindAsync([command.PackageId], ct);
        
        if (package == null)
        {
            logger.LogWarning("{Message}: Fetching Package with ID {PackageId} not found", LoggingMessages.NotFound, command.PackageId);
            throw new BusinessException(LoggingMessages.NotFound, $"Package {command.PackageId} not found", HttpStatusCode.NotFound);
        }

        // 2. Pre-condition: Check for active enrollment
        var hasActiveEnrollment = await db.Enrollments
            .AnyAsync(e => e.LeadId == command.LeadId && e.EndDate >= command.StartDate && !e.IsDeleted, ct);

        if (hasActiveEnrollment)
        {
            logger.LogWarning("{Message}: Lead {LeadId} already has an active enrollment.", LoggingMessages.Conflict, command.LeadId);
            throw new BusinessException(LoggingMessages.Conflict, $"Lead {command.LeadId} already has an active enrollment.", HttpStatusCode.Conflict);
        }

        // 3. Create RejoinRecord
        var rejoinRecord = new RejoinRecord
        {
            Id = Guid.NewGuid(),
            LeadId = lead.Id,
            PackageId = package.Id,
            StartDate = command.StartDate,
            EndDate = command.StartDate.AddDays(package.DurationInDays),
            PackageCostSnapshot = package.Cost,
            PackageDurationSnapshot = package.DurationInDays,
            CreatedAt = DateTime.UtcNow
        };

        // 4. Create Initial Bill
        var bill = new Bill
        {
            Id = Guid.NewGuid(),
            LeadId = lead.Id,
            RejoinRecordId = rejoinRecord.Id,
            InitialAmount = package.Cost,
            AmountPaid = 0, // No payment accepted at this simple endpoint step
            CreatedAt = DateTime.UtcNow
        };

        // wire relations up for EF Core graph addition
        rejoinRecord.Bill = bill;

        await db.RejoinRecords.AddAsync(rejoinRecord, ct);

        // 5. Update Lead Status
        lead.Status = LeadStatus.Converted;

        // 6. Cleanup Stale FollowUps
        foreach (var followUp in lead.FollowUps.Where(f => f.Status == FollowUpStatus.Pending))
        {
            followUp.Status = FollowUpStatus.Cancelled;
            followUp.Notes = string.IsNullOrEmpty(followUp.Notes) 
                ? "Automatically cancelled due to Rejoin." 
                : followUp.Notes + " [Automatically cancelled due to Rejoin.]";
        }

        // 7. Save
        await db.SaveChangesAsync(ct);

        logger.LogInformation("{Message}: RejoinRecord {RejoinRecordId} created.", 
            LoggingMessages.ResourceCreated, rejoinRecord.Id);

        return new CreateRejoinResponse(rejoinRecord.Id);
    }
}
