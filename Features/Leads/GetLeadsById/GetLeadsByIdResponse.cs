using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.API.Common.Enums;

namespace CRM.API.Features.Leads.GetLeadsById
{
    public record GetLeadsByIdResponse(
        Guid Id,
        string Name,
        string Phone,
        LeadStatus Status,
        string Source,
        string Reason,
        DateTime CreatedAt
    );

}