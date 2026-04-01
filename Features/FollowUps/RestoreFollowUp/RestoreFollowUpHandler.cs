using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.FollowUps.RestoreFollowUp
{
    public class RestoreFollowUpHandler(
        AppDbContext db,
        ILogger<RestoreFollowUpCommand> logger
    ) :
    IRequestHandler<RestoreFollowUpCommand, RestoreFollowUpResponse>
    {
        public async Task<RestoreFollowUpResponse> Handle(RestoreFollowUpCommand command, CancellationToken cancellationToken)
        {
            var followUp = await db.FollowUps
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(f => f.Id == command.Request.Id, cancellationToken);

            if (followUp == null)
            {
                throw new BusinessException(LoggingMessages.NotFound, $"Restoring Follow-Up with ID {command.Request.Id}");
            }

            followUp.IsDeleted = false;
            followUp.DeletedAt = null;

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Follow-Up with ID {FollowUpId} restored correctly.", command.Request.Id);

            return new RestoreFollowUpResponse(true);
        }
    }
}