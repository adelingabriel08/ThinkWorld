using MediatR;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Comment;

public record DeleteCommentCmd : IRequest<OperationResult<DeleteCommentCmd>>
{
    public Guid? CommentId { get; init; }
}