using MediatR;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Post;

public record GetPostsCmd(Guid? CommunityId, string Email) : IRequest<OperationResult<List<CommunityPost>>>;
