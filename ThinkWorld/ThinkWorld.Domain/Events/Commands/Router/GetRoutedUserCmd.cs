using MediatR;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Router;

public record GetRoutedUserCmd(string Email) : IRequest<OperationResult<RoutedUser>>;