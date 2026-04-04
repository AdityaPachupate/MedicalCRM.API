namespace CRM.API.Features.Enrollments.GetEnrollmentById
{
    public record GetEnrollmentByIdResponse(
        Guid Id,
        Guid LeadId,
        string LeadName,
        string LeadPhone,
        Guid PackageId,
        string PackageName,
        decimal PackageCostSnapshot,
        int PackageDurationSnapshot,
        DateOnly StartDate,
        DateOnly EndDate,
        DateTime CreatedAt,
        BillDto? Bill
    );

    public record BillDto(
        Guid Id,
        decimal PackageAmount,
        decimal AdvanceAmount,
        decimal PendingAmount,
        decimal MedicineBillingAmount,
        DateTime CreatedAt
    );
}
