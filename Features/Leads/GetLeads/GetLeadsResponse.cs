namespace CRM.API.Features.Leads.GetLeads
{
    using CRM.API.Common.Enums;

    public record GetLeadsResponse(
    Guid Id,
    string Name,
    string Phone,
    LeadStatus Status,
    string Source,
    string Reason,
    DateTime CreatedAt
    );
}
