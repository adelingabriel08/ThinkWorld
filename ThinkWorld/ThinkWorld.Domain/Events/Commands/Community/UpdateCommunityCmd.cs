using MediatR;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Community;

public record UpdateCommunityCmd : IRequest<OperationResult<Aggregates.Community>>
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
}
