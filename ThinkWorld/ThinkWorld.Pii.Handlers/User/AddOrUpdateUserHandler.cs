using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Events.Commands.User;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Pii.Handlers.User;

public class AddOrUpdateUserHandler : IRequestHandler<AddOrUpdateUserCmd, OperationResult<Domain.Aggregates.User>>
{
    private readonly UserDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public AddOrUpdateUserHandler(UserDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult<Domain.Aggregates.User>> Handle(AddOrUpdateUserCmd request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return OperationResult<Domain.Aggregates.User>.Failed("Email cannot be null");
        }
        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            return OperationResult<Domain.Aggregates.User>.Failed("FirstName cannot be null");
        }
        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            return OperationResult<Domain.Aggregates.User>.Failed("LastName cannot be null");
        }
        
        var userId = _userIdGenerator.ComputeUserId(request.Email);
        
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);
        
        if (user == null)
        {
            user = new Domain.Aggregates.User
            {
                Id = userId,
                CreatedAt = DateTime.UtcNow,
                Annonymised = false,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                ImageUrl = request.ImageUrl,
            };
            
            await _context.Users.AddAsync(user, cancellationToken);
        }
        else
        {
            user.FirstName = request.FirstName;
            user.LastName = request.LastName; 
            user.ImageUrl = request.ImageUrl;
            user.UpdatedAt = DateTime.UtcNow;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult<Domain.Aggregates.User>.Succeeded(user);
    }
}