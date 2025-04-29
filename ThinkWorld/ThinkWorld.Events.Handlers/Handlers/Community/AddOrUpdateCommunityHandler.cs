using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Events.Commands.Community;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Events.Handlers.Handlers.Community;

public class AddOrUpdateCommunityHandler : IRequestHandler<AddOrUpdateCommunityCmd, OperationResult<Domain.Aggregates.Community>>
{
    private readonly CosmosDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public AddOrUpdateCommunityHandler(CosmosDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult<Domain.Aggregates.Community>> Handle(AddOrUpdateCommunityCmd request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return OperationResult<Domain.Aggregates.Community>.Failed("Name is required.");
        }
        
        if (string.IsNullOrEmpty(request.Description))
        {
            return OperationResult<Domain.Aggregates.Community>.Failed("Description is required.");
        }
        
        if (string.IsNullOrEmpty(request.Email))
        {
            return OperationResult<Domain.Aggregates.Community>.Failed("Email is required.");
        }
        
        if (string.IsNullOrEmpty(request.ImageUrl))
        {
            return OperationResult<Domain.Aggregates.Community>.Failed("An image is required.");
        }
        
        var userId = _userIdGenerator.ComputeUserIdAsync(request.Email);
        
        if (request.Id != null)
        {
            var community = await _context.Communities
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            
            if (community == null || community.CreatedBy != userId)
            {
                return OperationResult<Domain.Aggregates.Community>.Failed("Community not found.");
            }
            
            community.Name = request.Name;
            community.Description = request.Description;
            community.ImageUrl = request.ImageUrl;
            community.UpdatedAt = DateTime.UtcNow;
            
            _context.Communities.Update(community);
            await _context.SaveChangesAsync(cancellationToken);
            
            return OperationResult<Domain.Aggregates.Community>.Succeeded(community);
        }
        
        var newCommunity = new Domain.Aggregates.Community
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
        };
        
        await _context.Communities.AddAsync(newCommunity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult<Domain.Aggregates.Community>.Succeeded(newCommunity);
        
    }
}