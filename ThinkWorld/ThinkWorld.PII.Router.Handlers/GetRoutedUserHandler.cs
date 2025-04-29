using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Entities.Router;
using ThinkWorld.Domain.Events.Commands.Router;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.PII.Router.Handlers;

public class GetRoutedUserHandler : IRequestHandler<GetRoutedUserCmd, OperationResult<RoutedUser>>
{
    private readonly RouterDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public GetRoutedUserHandler(RouterDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult<RoutedUser>> Handle(GetRoutedUserCmd request, CancellationToken cancellationToken)
    {
        var userId = _userIdGenerator.ComputeUserIdAsync(request.Email);
        
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return OperationResult<RoutedUser>.Succeeded(user);
    }
}