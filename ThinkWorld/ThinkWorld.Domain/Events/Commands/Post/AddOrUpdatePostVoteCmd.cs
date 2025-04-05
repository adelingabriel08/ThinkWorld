using MediatR;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Post;

public record AddOrUpdatePostVoteCmd : IRequest<OperationResult<PostVote>>
{
    public Guid? PostId { get; init; }
    public bool? IsUpvote { get; init; }
}