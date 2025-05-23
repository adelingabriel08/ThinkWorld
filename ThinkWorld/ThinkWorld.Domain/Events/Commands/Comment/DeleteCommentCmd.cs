using MediatR;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Comment;

public record DeleteCommentCmd(Guid CommentId, string Email) : IRequest<OperationResult<DeleteCommentCmd>>;
