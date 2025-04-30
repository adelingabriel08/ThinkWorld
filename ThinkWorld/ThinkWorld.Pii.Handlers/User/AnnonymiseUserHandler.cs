using MediatR;
using ThinkWorld.Domain.Events.Commands.User;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Pii.Handlers.User;

public class AnnonymiseUserHandler : IRequestHandler<AnnonymiseUserCmd, OperationResult>
{
    private readonly UserDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public AnnonymiseUserHandler(UserDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult> Handle(AnnonymiseUserCmd request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return OperationResult.Failed("Email cannot be null");
        }
        
        var userId = _userIdGenerator.ComputeUserId(request.Email);
        
        var user = _context.Users
            .FirstOrDefault(x => x.Id == userId);
        
        if (user == null) 
            return OperationResult.Failed("User not found");
        
        var newUser = new Domain.Aggregates.User
        {
            Id = userId,
            CreatedAt = user.CreatedAt,
            AnnonymisedAt = DateTime.UtcNow,
            Annonymised = true,
        };
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        
        return OperationResult.Succeeded();
            
    }
}