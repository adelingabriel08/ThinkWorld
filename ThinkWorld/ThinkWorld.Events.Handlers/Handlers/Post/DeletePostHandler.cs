using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Events.Commands.Post;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Events.Handlers.Handlers.Post;

public class DeletePostHandler : IRequestHandler<DeletePostCmd, OperationResult>
{
    private readonly CosmosDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public DeletePostHandler(CosmosDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult> Handle(DeletePostCmd request, CancellationToken cancellationToken)
    {
        if (request.PostId == Guid.Empty)
        {
            return OperationResult.Failed("Post ID cannot be null.");
        }
        
        if (request.CommunityId == Guid.Empty)
        {
            return OperationResult.Failed("CommunityId cannot be empty.");
        }

        if (string.IsNullOrEmpty(request.Email))
        {
            return OperationResult.Failed("Email cannot be empty.");
        }
        
        var userId = _userIdGenerator.ComputeUserId(request.Email);
        
        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.Id == request.PostId && request.CommunityId == p.CommunityId, cancellationToken);
        
        if (post == null)
            return OperationResult.Succeeded(); // handle retries

        if (post.CreatedBy != userId)
        {
            return OperationResult.Failed("You are not the owner of this post.");
        }
        
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync(cancellationToken);
        
        return OperationResult.Succeeded();
            
    }
}