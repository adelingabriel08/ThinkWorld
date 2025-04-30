using MediatR;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.User;

public record AnnonymiseUserCmd(string Email) : IRequest<OperationResult>;