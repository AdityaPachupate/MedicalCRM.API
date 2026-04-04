using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Domain;
using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CRM.API.Features.Enrollments.UpdateEnrollment
{
    public class UpdateEnrollmentHandler(
        AppDbContext db,
        ILogger<UpdateEnrollmentHandler> logger
    ) : IRequestHandler<UpdateEnrollmentCommand, UpdateEnrollmentResponse>
    {
        public async Task<UpdateEnrollmentResponse> Handle(UpdateEnrollmentCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var enrollment = await db.Enrollments
                    .Include(e => e.Package)
                    .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (enrollment == null)
            {
                throw new BusinessException(
                   LoggingMessages.NotFound,
                   $"Updating enrollment with ID {request.Id}",
                   HttpStatusCode.NotFound
               );
            }


            if (request.LeadId.HasValue && request.LeadId != enrollment.LeadId)
            {
                enrollment.LeadId = request.LeadId.Value;
            }

            bool packageChanged = false;
            if (request.PackageId.HasValue && request.PackageId != enrollment.PackageId)
            {
                var newPackage = await db.Packages
                    .FirstOrDefaultAsync(p => p.Id == request.PackageId.Value, cancellationToken);

                if (newPackage == null)
                {
                    throw new BusinessException(
                        LoggingMessages.NotFound,
                        $"Package with ID {request.PackageId.Value} not found",
                        HttpStatusCode.NotFound
                    );
                }

                enrollment.PackageId = newPackage.Id;
                enrollment.PackageCostSnapshot = newPackage.Cost;
                enrollment.PackageDurationSnapshot = newPackage.DurationInDays;
                packageChanged = true;
            }

            // Manual Snapshot Overrides
            enrollment.PackageCostSnapshot = request.PackageCostSnapshot ?? enrollment.PackageCostSnapshot;
            enrollment.PackageDurationSnapshot = request.PackageDurationSnapshot ?? enrollment.PackageDurationSnapshot;

            // Dates
            bool startDateChanged = request.StartDate.HasValue && request.StartDate != enrollment.StartDate;
            enrollment.StartDate = request.StartDate ?? enrollment.StartDate;

            if (request.EndDate.HasValue)
            {
                enrollment.EndDate = request.EndDate.Value;
            }
            else if (packageChanged || startDateChanged)
            {
                // Recalculate EndDate based on (potentially new) StartDate and (potentially new) Package Duration
                enrollment.EndDate = enrollment.StartDate.AddDays(enrollment.PackageDurationSnapshot);
            }

            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation("{Message}: {EnrollmentId}", LoggingMessages.ResourceUpdated, enrollment.Id);

            return enrollment.Adapt<UpdateEnrollmentResponse>();
        }
    }
}