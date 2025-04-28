using MediatR;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Router;

public record AddOrUpdateRoutedCommentCmd : IRequest<OperationResult<RoutedUser>>
{
    public string? Id { get; init; }
    public string UserId { get; init; } = null!;
    public string PostId { get; init; } = null!;
    public Guid RegionId { get; init; }
}