using MediatR;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Router;

public record AddOrUpdateRoutedUserCmd(string Email, Guid RegionId) : IRequest<OperationResult<RoutedUser>>;