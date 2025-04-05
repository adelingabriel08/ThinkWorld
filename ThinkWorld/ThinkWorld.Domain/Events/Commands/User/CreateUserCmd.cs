using MediatR;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.User;

public record CreateUserCmd : IRequest<OperationResult<Aggregates.User>>
{
    public string? Email { get; init; } = string.Empty;
    public string? FirstName { get; init; } = string.Empty;
    public string? LastName { get; init; } = string.Empty;
}