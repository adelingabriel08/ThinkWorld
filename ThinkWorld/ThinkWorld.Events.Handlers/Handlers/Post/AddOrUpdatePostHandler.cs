using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Commands.Post;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Events.Handlers.Handlers.Post;

public class AddOrUpdatePostHandler : IRequestHandler<AddOrUpdatePostCmd, OperationResult<CommunityPost>>
{
    private readonly CosmosDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public AddOrUpdatePostHandler(CosmosDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult<CommunityPost>> Handle(AddOrUpdatePostCmd request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Title))
        {
            return OperationResult<CommunityPost>.Failed("Title cannot be empty");
        }
        
        if (string.IsNullOrEmpty(request.Content))
        {
            return OperationResult<CommunityPost>.Failed("Content cannot be empty");
        }
        
        if (string.IsNullOrEmpty(request.Email))
        {
            return OperationResult<CommunityPost>.Failed("Email cannot be empty");
        }
        
        if (Guid.Empty == request.CommunityId)
        {
            return OperationResult<CommunityPost>.Failed("Id cannot be empty");
        }
        
        // TODO add check for joined communities
        
        var userId = _userIdGenerator.ComputeUserId(request.Email);
        
        var community = await _context.Communities
            .FirstOrDefaultAsync(c => c.Id == request.CommunityId, cancellationToken);
        
        if (community == null)
        {
            return OperationResult<CommunityPost>.Failed("Community not found");
        }

        if (request.Id != null)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == request.Id && p.CommunityId == request.CommunityId, cancellationToken);

            if (post == null)
            {
                return OperationResult<CommunityPost>.Failed("Post not found");
            }
            
            post.Title = request.Title;
            post.Content = request.Content;
            post.UpdatedAt = DateTime.UtcNow;
            post.UpdatedBy = userId;
            post.ImageUrl = request.ImageUrl;
            
            _context.Posts.Update(post);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<CommunityPost>.Succeeded(post);
        }
        
        
        var newPost = new CommunityPost
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            CommunityId = request.CommunityId,
            CreatedBy = userId
        };
        
        await _context.Posts.AddAsync(newPost, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return OperationResult<CommunityPost>.Succeeded(newPost);
    }
}