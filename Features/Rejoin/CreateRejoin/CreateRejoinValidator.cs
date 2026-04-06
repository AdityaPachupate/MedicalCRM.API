using FluentValidation;

namespace CRM.API.Features.Rejoin.CreateRejoin;

public class CreateRejoinValidator : AbstractValidator<CreateRejoinCommand>
{
    public CreateRejoinValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
        RuleFor(x => x.PackageId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
    }
}
