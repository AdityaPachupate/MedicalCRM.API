using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CRM.API.Features.Rejoin.DeleteRejoin;

public class DeleteRejoinHandler(AppDbContext db, ILogger<DeleteRejoinHandler> logger) 
    : IRequestHandler<DeleteRejoinCommand, DeleteRejoinResponse>
{
    public async Task<DeleteRejoinResponse> Handle(DeleteRejoinCommand command, CancellationToken ct)
    {
        var record = await db.RejoinRecords.FindAsync([command.Id], ct);
        if (record == null)
        {
            logger.LogWarning("{Message}: RejoinRecord {Id} not found.", LoggingMessages.NotFound, command.Id);
            throw new BusinessException(LoggingMessages.NotFound, $"RejoinRecord {command.Id} not found.", HttpStatusCode.NotFound);
        }

        if (record.IsDeleted)
        {
            logger.LogWarning("{Message}: RejoinRecord {Id} is already deleted.", LoggingMessages.ValidationFailed, command.Id);
            throw new BusinessException(LoggingMessages.ValidationFailed, $"RejoinRecord {command.Id} is already deleted.", HttpStatusCode.BadRequest);
        }

        if (command.IsPermanent)
        {
            // Hard Delete
            db.RejoinRecords.Remove(record);
            logger.LogInformation("{Message}: RejoinRecord {Id} hard deleted.", LoggingMessages.ResourceUpdated, record.Id);
        }
        else
        {
            // Soft Delete
            record.IsDeleted = true;
            record.DeletedAt = DateTime.UtcNow;

            // Cascade Soft Delete
            await db.Bills
                .Where(b => b.RejoinRecordId == record.Id && !b.IsDeleted)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.IsDeleted, true)
                    .SetProperty(b => b.DeletedAt, DateTime.UtcNow), ct);

            logger.LogInformation("{Message}: RejoinRecord {Id} and related Bill soft deleted.", LoggingMessages.ResourceUpdated, record.Id);
        }

        return new DeleteRejoinResponse(true);
    }
}
