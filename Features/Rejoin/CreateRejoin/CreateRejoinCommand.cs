using MediatR;

namespace CRM.API.Features.Rejoin.CreateRejoin;

public record CreateRejoinCommand(
    Guid LeadId,
    Guid PackageId,
    DateOnly StartDate
) : IRequest<CreateRejoinResponse>;
