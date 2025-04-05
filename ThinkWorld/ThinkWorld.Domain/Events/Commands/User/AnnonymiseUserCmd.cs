using MediatR;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.User;

public record AnnonymiseUserCmd : IRequest<OperationResult<Aggregates.User>>
{
    public Guid? UserId { get; init; }
}