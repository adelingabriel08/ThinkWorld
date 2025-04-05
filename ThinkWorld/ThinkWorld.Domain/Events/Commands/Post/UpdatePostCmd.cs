using MediatR;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Post;

public record UpdatePostCmd : IRequest<OperationResult<CommunityPost>>
{
    public Guid? Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
}