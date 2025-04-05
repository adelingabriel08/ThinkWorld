using MediatR;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.User;

public record UpdateUserInfoCmd : IRequest<OperationResult<Aggregates.User>>
{
    public Guid? UserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? ImageUrl { get; init; }
}