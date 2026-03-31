using MediatR;

namespace CRM.API.Features.Leads.RestoreLead;

public record RestoreLeadRequest(Guid Id);

public record RestoreLeadResponse(bool Success);

public record RestoreLeadCommand(RestoreLeadRequest Request) : IRequest<RestoreLeadResponse>;
