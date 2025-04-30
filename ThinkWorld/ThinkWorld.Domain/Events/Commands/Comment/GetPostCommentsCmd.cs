using MediatR;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Comment;

public record GetPostCommentsCmd(Guid PostId) : IRequest<OperationResult<List<PostComment>>>;