using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.API.Features.Enrollments.UpdateEnrollment
{
    public record UpdateEnrollmentResponse(
        Guid Id,
        Guid? LeadId = null,
        Guid? PackageId = null,
        DateOnly? StartDate = null,
        DateOnly? EndDate = null,
        decimal? PackageCostSnapshot = null,
        int? PackageDurationSnapshot = null
    );
}