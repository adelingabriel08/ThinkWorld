using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Events.Commands.User;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Pii.Handlers.User;

public class GetUserInfoHandler : IRequestHandler<GetUserInfoCmd, OperationResult<Domain.Aggregates.User>>
{
    private readonly UserDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public GetUserInfoHandler(UserDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult<Domain.Aggregates.User>> Handle(GetUserInfoCmd request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return OperationResult<Domain.Aggregates.User>.Failed("Email cannot be null");
        }
        
        var userId = _userIdGenerator.ComputeUserId(request.Email);
        
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        
        return OperationResult<Domain.Aggregates.User>.Succeeded(user);
    }
}