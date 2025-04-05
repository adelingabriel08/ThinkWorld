using MediatR;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Post;

public record DeletePostCmd : IRequest<OperationResult<DeletePostCmd>>
{
    public Guid? PostId { get; init; }
}