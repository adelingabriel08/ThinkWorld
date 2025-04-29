using MediatR;
using ThinkWorld.Domain.Events.Results;

namespace ThinkWorld.Domain.Events.Commands.Community;

public record GetCommunitiesCmd() : IRequest<OperationResult<List<Aggregates.Community>>>;