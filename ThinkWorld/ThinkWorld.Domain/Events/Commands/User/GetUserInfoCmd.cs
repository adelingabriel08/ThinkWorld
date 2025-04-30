using MediatR;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.User;

public record GetUserInfoCmd(string Email) : IRequest<OperationResult<Aggregates.User>>;