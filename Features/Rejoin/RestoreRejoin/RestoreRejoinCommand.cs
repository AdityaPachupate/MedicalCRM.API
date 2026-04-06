using System;
using MediatR;

namespace CRM.API.Features.Rejoin.RestoreRejoin;

public record RestoreRejoinCommand(Guid Id) : IRequest<RestoreRejoinResponse>;
