using FluentValidation;

namespace CRM.API.Features.Enrollments.CreateEnrollment
{
    public class CreateEnrollmentValidator : AbstractValidator<CreateEnrollmentRequest>
    {
        public CreateEnrollmentValidator()
        {
            RuleFor(x => x.LeadId).NotEmpty();
            RuleFor(x => x.PackageId).NotEmpty();
            RuleFor(x => x.StartDate).NotEmpty();
        }
    }
}