using CRM.API.Common.Enums;
using CRM.API.Common.Models;
using CRM.API.Domain;
using MediatR;

namespace CRM.API.Features.Leads.GetLeads
{
    public record GetLeadsQuery(LeadStatus? Status, string? Source, int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<GetLeadsResponse>>
    {
    }
}
