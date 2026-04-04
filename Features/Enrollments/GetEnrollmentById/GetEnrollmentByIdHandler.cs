using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CRM.API.Features.Enrollments.GetEnrollmentById
{
    public class GetEnrollmentByIdHandler(AppDbContext db) : IRequestHandler<GetEnrollmentByIdQuery, GetEnrollmentByIdResponse>
    {
        public async Task<GetEnrollmentByIdResponse> Handle(GetEnrollmentByIdQuery query, CancellationToken cancellationToken)
        {
            var enrollment = await db.Enrollments
                .AsNoTracking()
                .IgnoreQueryFilters() // Allow fetching trashed items if ID is known
                .Where(e => e.Id == query.Id)
                .ProjectToType<GetEnrollmentByIdResponse>()
                .FirstOrDefaultAsync(cancellationToken);

            if (enrollment == null)
            {
                throw new BusinessException(
                    LoggingMessages.NotFound,
                    $"Fetching enrollment with ID {query.Id}",
                    HttpStatusCode.NotFound
                );
            }

            return enrollment;
        }
    }
}
