using System;
using MediatR;

namespace CRM.API.Features.Rejoin.DeleteRejoin;

public record DeleteRejoinCommand(Guid Id, bool IsPermanent = false) : IRequest<DeleteRejoinResponse>;
