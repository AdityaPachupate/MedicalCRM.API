using MediatR;
using System.Collections.Generic;

namespace CRM.API.Features.Lookups.GetLookups
{
    public record GetLookupsQuery(string? Category) : IRequest<List<GetLookupsResponse>>;
}
