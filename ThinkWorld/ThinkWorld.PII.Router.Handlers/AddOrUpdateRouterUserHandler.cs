using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Commands.Router;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.PII.Router.Handlers;

public class AddOrUpdateRouterUserHandler : IRequestHandler<AddOrUpdateRoutedUserCmd, OperationResult<RoutedUser>>
{
    private readonly RouterDbContext _routerDbContext;
    private readonly IUserIdGenerator _userIdGenerator;

    public AddOrUpdateRouterUserHandler(RouterDbContext routerDbContext, IUserIdGenerator userIdGenerator)
    {
        _routerDbContext = routerDbContext;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult<RoutedUser>> Handle(AddOrUpdateRoutedUserCmd request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            return OperationResult<RoutedUser>.Failed("Email is required.");
        }
        
        if (Guid.Empty == request.RegionId)
        {
            return OperationResult<RoutedUser>.Failed("RegionId is required.");
        }
        
        // point read
        var region = await _routerDbContext.Regions
            .FirstOrDefaultAsync(r => r.Id == request.RegionId, cancellationToken);
        
        if (region == null)
        {
            return OperationResult<RoutedUser>.Failed("Region not found.");
        }
        
        var userKey = _userIdGenerator.ComputeUserId(request.Email);
        
        var user = await _routerDbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userKey, cancellationToken);
        
        if (user == null)
        {
            user = new RoutedUser
            {
                Id = userKey,
                RegionId = request.RegionId,
                CreatedAt = DateTime.UtcNow,
            };
            
            await _routerDbContext.Users.AddAsync(user, cancellationToken);
        }
        else
        {
            user.RegionId = request.RegionId;
            user.UpdatedAt = DateTime.UtcNow;
            
            _routerDbContext.Users.Update(user);
        }

        await _routerDbContext.SaveChangesAsync(cancellationToken);
        
        return OperationResult<RoutedUser>.Succeeded(user);
    }
}