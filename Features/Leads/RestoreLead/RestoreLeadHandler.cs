using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.Leads.RestoreLead;

public class RestoreLeadHandler(AppDbContext db, ILogger<RestoreLeadHandler> logger) 
    : IRequestHandler<RestoreLeadCommand, RestoreLeadResponse>
{
    public async Task<RestoreLeadResponse> Handle(RestoreLeadCommand command, CancellationToken cancellationToken)
    {
        // 1. Find the lead (must use IgnoreQueryFilters since it's "deleted")
        var lead = await db.Leads
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(l => l.Id == command.Request.Id, cancellationToken);

        if (lead == null)
            throw new BusinessException(LoggingMessages.NotFound, "Restoring Lead");

        // 2. Bring it back from the trash
        lead.IsDeleted = false;
        lead.DeletedAt = null;

        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Lead with ID {LeadId} restored correctly.", command.Request.Id);

        return new RestoreLeadResponse(true);
    }
}
