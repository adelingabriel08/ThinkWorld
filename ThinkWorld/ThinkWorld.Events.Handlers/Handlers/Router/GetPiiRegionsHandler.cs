using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Commands.Router;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Events.Handlers.Handlers.Router;

public class GetPiiRegionsHandler : IRequestHandler<GetPiiRegionsCmd, OperationResult<List<RoutingRegion>>>
{
    private readonly RouterDbContext _context;

    public GetPiiRegionsHandler(RouterDbContext context)
    {
        _context = context;
    }

    public async Task<OperationResult<List<RoutingRegion>>> Handle(GetPiiRegionsCmd request, CancellationToken cancellationToken)
    {
        var regions = await _context.Regions
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        
        return OperationResult<List<RoutingRegion>>.Succeeded(regions);
    }
}