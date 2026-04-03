namespace CRM.API.Features.Enrollments.CreateEnrollment
{
    public record CreateEnrollmentResponse(
        Guid Id,
        Guid LeadId,
        Guid PackageId,
        DateOnly StartDate,
        DateOnly EndDate,
        decimal PackageCostSnapshot,
        int PackageDurationSnapshot,
        DateTime CreatedAt
    );
}
