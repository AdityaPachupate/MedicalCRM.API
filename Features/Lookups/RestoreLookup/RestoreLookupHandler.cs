using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CRM.API.Features.Lookups.RestoreLookup
{
    public class RestoreLookupHandler(AppDbContext db) : IRequestHandler<RestoreLookupCommand, RestoreLookupResponse>
    {
        public async Task<RestoreLookupResponse> Handle(RestoreLookupCommand command, CancellationToken ct)
        {
            var lookup = await db.LookupValues
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Id == command.Id, ct);

            if (lookup == null)
            {
                throw new BusinessException(
                    LoggingMessages.NotFound,
                    $"Lookup with ID {command.Id} not found.",
                    System.Net.HttpStatusCode.NotFound
                );
            }

            if (!lookup.IsDeleted)
            {
                throw new BusinessException(
                    LoggingMessages.ValidationFailed,
                    $"Lookup with ID {command.Id} is not deleted.",
                    System.Net.HttpStatusCode.BadRequest
                );
            }

            lookup.IsDeleted = false;
            lookup.DeletedAt = null;

            await db.SaveChangesAsync(ct);
            return new RestoreLookupResponse(true);
        }
    }
}
