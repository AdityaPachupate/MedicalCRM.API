using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.Leads.DeleteLead
{
    public class DeleteLeadHandler(
        AppDbContext db,
        ILogger<DeleteLeadHandler> logger
    ) : IRequestHandler<DeleteLeadCommand, DeleteLeadResponse>
    {
        public async Task<DeleteLeadResponse> Handle(DeleteLeadCommand command, CancellationToken cancellationToken)
        {
            var lead = await db.Leads
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(l => l.Id == command.Request.Id, cancellationToken);

            if (lead == null)
            {
                throw new BusinessException(
                   LoggingMessages.NotFound,
                   $"Deleting lead with ID {command.Request.Id}",
                   System.Net.HttpStatusCode.NotFound
               );

            }

            if (command.IsPermanent)
            {
                // Hard Delete (Remove from DB permanently)
                db.Leads.Remove(lead);
                logger.LogInformation(
                "Lead with ID {LeadId} deleted successfully",
                command.Request.Id
            );
            }
            else
            {
                // Soft Delete (Move to Trash)
                lead.IsDeleted = true;
                lead.DeletedAt = DateTime.UtcNow;
                logger.LogInformation(
                "Lead with ID {LeadId} moved to Trash",
                command.Request.Id
            );
            }
            await db.SaveChangesAsync(cancellationToken);
            return new DeleteLeadResponse(true);
        }
    }
}