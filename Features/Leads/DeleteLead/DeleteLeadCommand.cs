using MediatR;

namespace CRM.API.Features.Leads.DeleteLead
{
    public record DeleteLeadCommand(DeleteLeadRequest Request) : IRequest<DeleteLeadResponse>
    {

    }
}