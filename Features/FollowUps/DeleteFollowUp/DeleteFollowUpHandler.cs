using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.FollowUps.DeleteFollowUp
{
    public class DeleteFollowUpHandler(
        AppDbContext db,
        ILogger<DeleteFollowUpCommand> logger
    ) : IRequestHandler<DeleteFollowUpCommand, DeleteFollowUpResponse>
    {
        public async Task<DeleteFollowUpResponse> Handle(DeleteFollowUpCommand command, CancellationToken cancellationToken)
        {
            var followUp = await db.FollowUps
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(f => f.Id == command.Request.Id, cancellationToken);

            if (followUp == null)
            {
                throw new BusinessException(
                   LoggingMessages.NotFound,
                   $"Deleting FollowUp with ID {command.Request.Id}",
                   System.Net.HttpStatusCode.NotFound
               );
            }

            if (command.IsPermanent)
            {
                db.Remove(followUp);

                logger.LogInformation(
                "FollowUp with ID {FollowUpId} deleted successfully",
                command.Request.Id);
            }
            else
            {
                followUp.IsDeleted = true;
                followUp.DeletedAt = DateTime.UtcNow;

                logger.LogInformation(
                "FollowUp with ID {FollowUpId} moved to Trash",
                command.Request.Id);
            }

            await db.SaveChangesAsync(cancellationToken);
            return new DeleteFollowUpResponse(true);
        }
    }
}