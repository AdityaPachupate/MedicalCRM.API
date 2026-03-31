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
                                .FirstOrDefaultAsync(l => l.Id == command.Request.Id);

            if (lead == null)
            {
                throw new BusinessException(
                   LoggingMessages.NotFound,
                   $"Deleting lead with ID {command.Request.Id}",
                   System.Net.HttpStatusCode.NotFound
               );

            }


            db.Leads.Remove(lead);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Lead with ID {LeadId} deleted successfully",
                command.Request.Id
            );

            return new DeleteLeadResponse(true);
        }
    }
}