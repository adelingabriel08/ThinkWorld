using MediatR;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Router;

public record CreateRoutedCommentCmd : IRequest<OperationResult<RoutedComment>>
{
    public string Id { get; init; } = null!;
    public string UserId { get; init; } = null!;
    public string PostId { get; init; } = null!;
    public Guid RegionId { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public record GetRoutedCommentsByPostIdCmd : IRequest<OperationResult<List<RoutedComment>>>
{
    public string PostId { get; init; } = null!;
}
