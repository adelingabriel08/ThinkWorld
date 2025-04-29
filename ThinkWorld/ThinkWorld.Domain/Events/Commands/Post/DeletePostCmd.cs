using MediatR;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Post;

public record DeletePostCmd(Guid PostId, Guid CommunityId, string Email) : IRequest<OperationResult>;