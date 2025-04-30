using MediatR;
using Microsoft.EntityFrameworkCore;
using ThinkWorld.Domain.Aggregates;
using ThinkWorld.Domain.Events.Commands.Post;
using ThinkWorld.Domain.Events.Results;
using ThinkWorld.Services;
using ThinkWorld.Services.DataContext;

namespace ThinkWorld.Events.Handlers.Handlers.Post;

public class GetPostsHandler : IRequestHandler<GetPostsCmd, OperationResult<List<CommunityPost>>>
{
    private readonly CosmosDbContext _context;
    private readonly IUserIdGenerator _userIdGenerator;

    public GetPostsHandler(CosmosDbContext context, IUserIdGenerator userIdGenerator)
    {
        _context = context;
        _userIdGenerator = userIdGenerator;
    }

    public async Task<OperationResult<List<CommunityPost>>> Handle(GetPostsCmd request, CancellationToken cancellationToken)
    {
        // TODO check joined communities
        
        if (string.IsNullOrEmpty(request.Email))
        {
            return OperationResult<List<CommunityPost>>.Failed("Email cannot be empty");
        }
        
        if (request.CommunityId != null && request.CommunityId == Guid.Empty)
        {
            return OperationResult<List<CommunityPost>>.Failed("CommunityId cannot be empty");
        }
        
        var userId = _userIdGenerator.ComputeUserId(request.Email);
        
        var query = _context.Posts
            .AsQueryable();
        
        if (request.CommunityId != null)
            query = query.Where(p => request.CommunityId == p.CommunityId);
        
        var posts = await query.ToListAsync(cancellationToken);
        
        return OperationResult<List<CommunityPost>>.Succeeded(posts);
    }
}