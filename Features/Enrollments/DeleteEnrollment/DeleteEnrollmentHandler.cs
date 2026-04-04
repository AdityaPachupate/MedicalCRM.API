using CRM.API.Common.Constants;
using CRM.API.Common.ExceptionHandling;
using CRM.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Features.Enrollments.DeleteEnrollment
{
    public class DeleteEnrollmentHandler(
        AppDbContext db,
        ILogger<DeleteEnrollmentHandler> logger
    ) : IRequestHandler<DeleteEnrollmentCommand, DeleteEnrollmentResponse>
    {
        public async Task<DeleteEnrollmentResponse> Handle(DeleteEnrollmentCommand command, CancellationToken cancellationToken)
        {
            var enrollment = await db.Enrollments
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(e => e.Id == command.Request.Id, cancellationToken);

            if (enrollment == null)
            {
                throw new BusinessException(
                   LoggingMessages.NotFound,
                   $"Deleting enrollment with ID {command.Request.Id}",
                   System.Net.HttpStatusCode.NotFound
               );
            }

            if (command.IsPermanent)
            {
                // Hard Delete
                db.Enrollments.Remove(enrollment);
                logger.LogInformation("Enrollment with ID {EnrollmentId} deleted permanently", command.Request.Id);
            }
            else
            {
                // Soft Delete
                enrollment.IsDeleted = true;
                enrollment.DeletedAt = DateTime.UtcNow;
                logger.LogInformation("Enrollment with ID {EnrollmentId} moved to Trash", command.Request.Id);
            }

            await db.SaveChangesAsync(cancellationToken);
            return new DeleteEnrollmentResponse(true);
        }
    }
}
