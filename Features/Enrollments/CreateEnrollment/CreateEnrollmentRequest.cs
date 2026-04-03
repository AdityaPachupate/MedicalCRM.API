namespace CRM.API.Features.Enrollments.CreateEnrollment
{
    public record CreateEnrollmentRequest(
        Guid LeadId,
        Guid PackageId,
        DateOnly StartDate
    );
}