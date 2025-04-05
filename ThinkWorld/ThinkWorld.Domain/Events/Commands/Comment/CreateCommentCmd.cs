using MediatR;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Comment;

public record CreateCommentCmd : IRequest<OperationResult<PostComment>>
{
    public Guid? PostId { get; init; }
    public string? Content { get; init; }
}