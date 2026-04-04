using CRM.API.Common.Constants;
using CRM.API.Common.Enums;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Domain;
using CRM.API.Infrastructure.Persistence;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CRM.API.Features.Enrollments.CreateEnrollment
{
    public class CreateEnrollmentHandler(AppDbContext db, ILogger<CreateEnrollmentHandler> logger) 
        : IRequestHandler<CreateEnrollmentCommand, CreateEnrollmentResponse>
    {
        public async Task<CreateEnrollmentResponse> Handle(CreateEnrollmentCommand command, CancellationToken cancellationToken)
        {
            // 1. Fetch Related Entities
            var lead = await db.Leads.FindAsync([command.Request.LeadId], cancellationToken) 
                       ?? throw new BusinessException(LoggingMessages.NotFound, $"Lead with ID {command.Request.LeadId} not found", HttpStatusCode.NotFound);
            
            var package = await db.Packages.FindAsync([command.Request.PackageId], cancellationToken) 
                          ?? throw new BusinessException(LoggingMessages.NotFound, $"Package with ID {command.Request.PackageId} not found", HttpStatusCode.NotFound);

            // 2. Map and Calculate
            var enrollment = new Enrollment
            {
                LeadId = lead.Id,
                PackageId = package.Id,
                StartDate = command.Request.StartDate,
                EndDate = command.Request.StartDate.AddDays(package.DurationInDays),
                PackageCostSnapshot = package.Cost,
                PackageDurationSnapshot = package.DurationInDays,
                CreatedAt = DateTime.UtcNow
            };

            // 3. Update Lead Status
            lead.Status = LeadStatus.Converted;

            // 4. Save
            await db.Enrollments.AddAsync(enrollment, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "{Message}: Enrollment {EnrollmentId} created for Lead {LeadId}",
                LoggingMessages.ResourceCreated,
                enrollment.Id,
                lead.Id
            );

            return enrollment.Adapt<CreateEnrollmentResponse>();
        }
    }
}