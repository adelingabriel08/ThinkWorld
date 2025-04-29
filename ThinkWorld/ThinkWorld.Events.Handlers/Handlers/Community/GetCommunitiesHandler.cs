using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Events.Commands.Community;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Events.Handlers.Handlers.Community;

public class GetCommunitiesHandler : IRequestHandler<GetCommunitiesCmd, OperationResult<List<Domain.Aggregates.Community>>>
{
    private readonly CosmosDbContext _context;

    public GetCommunitiesHandler(CosmosDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<List<Domain.Aggregates.Community>>> Handle(GetCommunitiesCmd request, CancellationToken cancellationToken)
    {
        var communities = await _context.Communities
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return OperationResult<List<Domain.Aggregates.Community>>.Succeeded(communities);
    }
}