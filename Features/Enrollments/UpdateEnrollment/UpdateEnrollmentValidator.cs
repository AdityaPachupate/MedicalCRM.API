using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CRM.API.Features.Enrollments.UpdateEnrollment
{
    public class UpdateEnrollmentValidator : AbstractValidator<UpdateEnrollmentRequest>
    {
        public UpdateEnrollmentValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            
            RuleFor(x => x.PackageCostSnapshot)
                .GreaterThanOrEqualTo(0)
                .When(x => x.PackageCostSnapshot.HasValue);

            RuleFor(x => x.PackageDurationSnapshot)
                .GreaterThan(0)
                .When(x => x.PackageDurationSnapshot.HasValue);
        }
    }
}